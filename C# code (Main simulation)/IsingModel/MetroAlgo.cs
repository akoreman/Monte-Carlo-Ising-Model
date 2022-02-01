using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;

namespace IsingSimulation
{
    public class MonteCarloSimulation
    {
        // Declare some variables that are used throughout the class.
        string flipMethod;
        int latticeSize;
        int numberSampleTimeSteps;

        Random randomGenerator = new Random();

        ConcurrentBag<IsingLattice> latticeList;
   
        int numberEquilibrationTimeSteps;
        int timeStepSize;

        int numberBootstrapSamples;

        double[] temperatureRange;

        /// <summary>
        /// Constructor of the MonteCarloSim class.
        /// </summary>
        /// <param name="latticeSize">Size of one edge of the square lattice.</param>
        /// <param name="temperatureRangeStart">Lowest temperature in the range that we simulate.</param>
        /// <param name="temperatureRangeEnd">Lowest temperature in the range that we simulate.</param>
        /// <param name="temperatureInterval">Size of the temperature steps.</param>
        /// <param name="flipMethod">String that chooses the algorithm used to flip bits. Options are "Montecarlo" and "Heatbath".</param>
        /// <param name="numberSampleTimeSteps">Number of steps that we want to sample the system.</param>
        /// <param name="numberEquilibrationTimeSteps">Number of flips to attempt before starting to take samples.</param>
        /// <param name="timeStepSize">Interval between each sample.</param>
        public MonteCarloSimulation(int latticeSize, double temperatureRangeStart, double temperatureRangeEnd, double temperatureInterval, string flipMethod, int numberSampleTimeSteps, int numberEquilibrationTimeSteps, int timeStepSize, int numberBootstrapSamples)
        { 
            this.latticeSize = latticeSize;
            this.flipMethod = flipMethod;

            // Generate an array with each temperature step in the range.
            temperatureRange = new double[Convert.ToInt32(Math.Ceiling((temperatureRangeEnd - temperatureRangeStart) / temperatureInterval))];

            for (int i = 0; i < temperatureRange.Length; i++)
                temperatureRange[i] = temperatureRangeStart + i * temperatureInterval;

            this.numberSampleTimeSteps = numberSampleTimeSteps;
            this.numberEquilibrationTimeSteps = numberEquilibrationTimeSteps;

            this.timeStepSize = timeStepSize;

            this.numberBootstrapSamples = numberBootstrapSamples;
        }

        /// <summary>
        /// Method that functions to start the simulation by starting the main thread.
        /// </summary>
        public void Run()
        {
            Thread mainThread = new Thread(() => StartMain());
            mainThread.Start();           
        }

        /// <summary>
        /// Method to run the simulation at one specific temperature.
        /// </summary>
        /// <param name="temperature">The temperature for which this worker runs the simulation.</param>
        public void StartWorker(double temperature)
        {
            IsingLattice lattice = new IsingLattice(latticeSize, temperature);

            // Run the Monte Carlo algorithm untill a lattice has finished.
            do
            {
                int flipXPosition = randomGenerator.Next(0, lattice.latticeSize);
                int flipYPosition = randomGenerator.Next(0, lattice.latticeSize);

                lattice.FlipBit(flipXPosition, flipYPosition, flipMethod);

                // After equilibration start sampling the system each timestep.
                if (lattice.numberFlipAttempts > numberEquilibrationTimeSteps * timeStepSize && lattice.numberFlipAttempts % timeStepSize == 0)
                {
                    lattice.energySamples.Add(lattice.totalEnergy);
                    lattice.magnetisationSamples.Add(lattice.MagnetisationPerSpin());
                }

                if (lattice.numberFlipAttempts == numberEquilibrationTimeSteps + timeStepSize * numberSampleTimeSteps)
                    lattice.simulationFinished = true;

            } while (!lattice.simulationFinished);
            
            latticeList.Add(lattice);
          
            // To keep track of the simulation while it's running print the temperature when a worker is finished.
            Console.WriteLine(lattice.temperature);
        }


        /// <summary>
        /// Method used by the main thread to first spin up all the worker threads, wait untill those are completed 
        /// then calculate and print to file the observables.
        /// </summary>
        public void StartMain()
        {
            List<Thread> threadList = new List<Thread>();
            latticeList = new ConcurrentBag<IsingLattice>();

            // Create a worker thread to run the simulation at each temperature.
            foreach (double temperature in temperatureRange)
            {
                    Thread workerThread = new Thread(() => StartWorker(temperature));
                    threadList.Add(workerThread);
                    workerThread.Start();

                    workerThread.Join();
            }

            // Since latticeList is not necessarily ordered we generate 
            // a new list that is ordered by temperature.
            List<IsingLattice> orderedLatticeList = latticeList.OrderBy(i => i.temperature).ToList();

            // Calculate the observables using the definitions from the project description.
            Observables observables = new Observables(numberBootstrapSamples);

            double[,] magnetisationPerSpin = observables.Magnetisation(orderedLatticeList);
            double[,] specificHeatPerSpin = observables.SpecificHeat(orderedLatticeList);
            double[,] susceptibilityPerSpin = observables.Susceptibility(orderedLatticeList);

            // Write the calculated observables to .csv files.
            observables.WriteObservableToFile(magnetisationPerSpin, "MagnetisationPerSpin" + latticeSize);
            observables.WriteObservableToFile(specificHeatPerSpin, "SpecificHeatPerSpin" + latticeSize);
            observables.WriteObservableToFile(susceptibilityPerSpin, "SusceptibilityPerSpin" + latticeSize);

            // Define streamwriters to create .csv files with all the generated latices and the temperatures. 
            // Used to train the CNN.
            StreamWriter latticeStream = new StreamWriter("Lattices" + latticeSize + ".csv");
            StreamWriter temperatureStream = new StreamWriter("TemperatureLabels.csv");

            foreach (IsingLattice lattice in orderedLatticeList)
            {
                lattice.PrintLattice(latticeStream);
                lattice.PrintTemp(temperatureStream);
            }

            latticeStream.Close();
            temperatureStream.Close();           
        }
    }
}
