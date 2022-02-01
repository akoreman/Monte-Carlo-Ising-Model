using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Globalization;
using System.Threading;

namespace IsingSimulation
{
    /// <summary>
    /// Class used to calculate the observables and the errors at the range of temperatures.
    /// </summary>
    class Observables
    {
        Random randomGenerator;
        int numberBootstrapSamples;

        /// <summary>
        /// Constructor of the Observables class
        /// </summary>
        /// <param name="numBootstrapSamples"> Number of bootstrap resamples to take. </param>
        public Observables(int numBootstrapSamples)
        {
            randomGenerator = new Random();
            this.numberBootstrapSamples = numBootstrapSamples;
        }

        /// <summary>
        /// Method to bootstrap resample a list.
        /// </summary>
        /// <param name="samples"> The list to bootstrap. </param>
        /// <returns> The bootstrapped list. </returns>
        public List<double> BootstrapResample(List<double> samples)
        {
            List<double> bootstrap = new List<double>();

            for (int i = 0; i < samples.Count; i++)
                bootstrap.Add(samples[(int) (randomGenerator.NextDouble() * samples.Count)]);

            return bootstrap;
        }

        /// <summary>
        /// Method to calculate the variance of a list.
        /// </summary>
        /// <param name="input"> List to calculate the variance of. </param>
        /// <returns> The variance of the list. </returns>
        public double Variance(List<double> input)
        {
            List<double> squaredAverageList = new List<double>();

            for (int i = 0; i < input.Count; i++)
                squaredAverageList.Add(input[i] * input[i]);

            return ( squaredAverageList.Average() - input.Average() * input.Average() );
        }

        /// <summary>
        /// Method to calculate the magnetisation per spin and the error for the range of temperatures in the latticelist.
        /// </summary>
        /// <param name="LatticeList"> List of all the lattices to calculate the magnetisation per spin of. </param>
        /// <returns> List with the magnetisation per spin and the error for each temperature. </returns>
        public double[,] Magnetisation(List<IsingLattice> LatticeList)
        {
            double[,] magnetisation = new double[LatticeList.Count, 2];

            for (int i = 0; i < LatticeList.Count; i++)
            {
                magnetisation[i, 0] = LatticeList[i].magnetisationSamples.Average();

                List<double> bootstrap = new List<double>();

                for (int j = 0; j <= numberBootstrapSamples; j++)
                    bootstrap.Add(BootstrapResample(LatticeList[i].magnetisationSamples).Average());

                magnetisation[i, 1] = Math.Sqrt(Variance(bootstrap));
            }
       
            return magnetisation;
        }

        /// <summary>
        /// Method to calculate the specific heat per spin and the error for the range of temperatures in the latticelist.
        /// </summary>
        /// <param name="LatticeList"> List of all the lattices to calculate the specific heat per spin of. </param>
        /// <returns> List with the specific heat per spin and the error for each temperature. </returns>
        public double[,] SpecificHeat(List<IsingLattice> LatticeList)
        {
            double[,] specificHeat = new double[LatticeList.Count, 2];
            int numSpins = LatticeList[0].latticeSize * LatticeList[0].latticeSize;

            for (int i = 0; i < LatticeList.Count; i++)
            {
                specificHeat[i, 0] = 1 / (LatticeList[i].temperature * LatticeList[i].temperature * numSpins) * Variance(LatticeList[i].energySamples);

                List<double> bootstrap = new List<double>();

                for (int j = 0; j <= numberBootstrapSamples; j++)
                    bootstrap.Add(1 / (LatticeList[i].temperature * LatticeList[i].temperature * numSpins) * Variance( BootstrapResample( LatticeList[i].energySamples) ) );

                specificHeat[i , 1] = Math.Sqrt(Variance(bootstrap));
            }

            return specificHeat;
        }

        /// <summary>
        /// Method to calculate the susceptibility per spin and the error for the range of temperatures in the latticelist.
        /// </summary>
        /// <param name="LatticeList"> List of all the lattices to calculate the susceptibility per spin of. </param>
        /// <returns> List with the susceptibility per spin and the error for each temperature. </returns>
        public double[,] Susceptibility(List<IsingLattice> LatticeList)
        {
            double[,] Susceptibility = new double[LatticeList.Count, 2];
            int numSpins = LatticeList[0].latticeSize * LatticeList[0].latticeSize;

            for (int i = 0; i < LatticeList.Count; i++)
            {
                Susceptibility[i, 0] = (numSpins / LatticeList[i].temperature) * Variance(LatticeList[i].magnetisationSamples);

                List<double> bootstrap = new List<double>();

                for (int j = 0; j < 11; j++)
                    bootstrap.Add( (numSpins / LatticeList[i].temperature) * Variance( BootstrapResample( LatticeList[i].magnetisationSamples) ) );

                Susceptibility[i, 1] = Math.Sqrt(Variance(bootstrap));
            }

            return Susceptibility;
        }

        /// <summary>
        /// Method to write to a .csv file a list of a observables and the error at a range of temperatures.
        /// </summary>
        /// <param name="observable"> 2xN list of doubles, where N is the number of temperature steps. </param>
        /// <param name="fileName"> String to choose the filename of the .csv file. </param>
        public void WriteObservableToFile(double[,] observable, string fileName)
        {
            // Change the culture of the thread to make sure that . is used as the decimal instead of ,    .
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            using (StreamWriter writer = new StreamWriter(fileName + ".csv"))
                for (int i = 0; i < observable.Length/2; i++)
                    writer.WriteLine(observable[i, 0] + "," + observable[i, 1]);
        }

    }
}