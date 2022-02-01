using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace IsingSimulation
{
    /// <summary>
    /// Class to contain 1 square ising lattice. Stores the relevant variables and functions.
    /// </summary>
    public class IsingLattice
    {
        public int latticeSize;
        public double temperature;
        public int numberFlipAttempts;

        public double totalEnergy = 0;

        private int[,] lattice;

        public bool simulationFinished;

        public List<double> energySamples;
        public List<double> magnetisationSamples;
        
        Random randomGenerator;

        /// <summary>
        /// Constructor for the IsingSimulation class. Initializes all the variables and array. Calculates the total energy at this point.
        /// </summary>
        /// <param name="latticeSize"> Size of one edge of the lattice. </param>
        /// <param name="temperature"> Temperature for this specific lattice. </param>
        public IsingLattice(int latticeSize, double temperature)
        {
            this.latticeSize = latticeSize;
            this.temperature = temperature;

            numberFlipAttempts = 0;

            randomGenerator = new Random();

            lattice = new int[this.latticeSize, this.latticeSize];
            lattice = InitialiseLattice();

            simulationFinished = false;

            for (int i = 0; i < latticeSize; i++)
                for (int j = 0; j < latticeSize; j++)
                    totalEnergy += EnergyOneSpin(i, j);

            energySamples = new List<double>();
            magnetisationSamples = new List<double>();
        }

        /// <summary>
        /// Method to attempt to flip a spin at a specific location.
        /// </summary>
        /// <param name="xPosition"> x position of the attempt.</param>
        /// <param name="yPosition"> y position of the attempt. </param>
        /// <param name="flipMethod"> Method to attempt to flip the spin. Options: "Metropolis" and "Heatbath" </param>
        public void FlipBit(int xPosition, int yPosition, string flipMethod)
        {
            double energyBefore, energyAfter, energyDelta;

            numberFlipAttempts++;
            double boltzmannFactor = randomGenerator.NextDouble();

            switch (flipMethod)
            {
                // Try to flip a spin, if it reduces energy accept it, if not accept it with the probablity given in the report.If accepted update the energy of the lattice.
                case "Metropolis":
                    energyBefore = EnergyOneSpin(xPosition, yPosition);

                    lattice[xPosition, yPosition] *= -1;

                    energyAfter = EnergyOneSpin(xPosition, yPosition);

                    energyDelta = energyAfter - energyBefore;

                    if (energyDelta > 0 && Math.Exp(-1 * energyDelta / temperature) < boltzmannFactor)
                        lattice[xPosition, yPosition] *= -1;
                    else 
                        totalEnergy += energyDelta;

                    break;

                // Try to flip a spin, accept it with the probablity given in the report. If accepted update the energy of the lattice.
                case "Heatbath":
                    energyBefore = EnergyOneSpin(xPosition, yPosition);

                    lattice[xPosition, yPosition] *= -1;

                    energyAfter = EnergyOneSpin(xPosition, yPosition);

                    energyDelta = energyAfter - energyBefore;

                    if (boltzmannFactor < (1 / (1 + Math.Exp((-1 * energyDelta) / temperature))))
                        lattice[xPosition, yPosition] *= -1;
                    else
                        totalEnergy += energyDelta;

                    break;
            }
        }

        /// <summary>
        /// Method to calculate the energy contribution of a spin at a specific location.
        /// </summary>
        /// <param name="xPos"> x position to calculate the contribution of. </param>
        /// <param name="yPos"> y position to calculate the contribution of. </param>
        /// <returns> The energy contribution of the spin at position (x,y). </returns>
        public double EnergyOneSpin(int xPos, int yPos)
        {
            return -lattice[xPos, yPos] * (lattice[Modulo(xPos - 1, latticeSize), yPos] + lattice[Modulo(xPos + 1, latticeSize), yPos] + lattice[xPos, Modulo(yPos - 1, latticeSize)] + lattice[xPos, Modulo(yPos + 1, latticeSize)]);
        }

        /// <summary>
        /// Method to calculate the magnetisation per spin of a lattice.
        /// </summary>
        /// <returns> Magnetisation per spin for this lattice. </returns>
        public double MagnetisationPerSpin()
        {
            double magnetisation = 0;

            for (int i = 0; i < latticeSize; i++)
                for (int j = 0; j < latticeSize; j++)
                    magnetisation += lattice[i, j];

            return magnetisation / (latticeSize * latticeSize);
        }

        /// <summary>
        /// Method to initialise the ising lattice to all spins up (1).
        /// </summary>
        int[,] InitialiseLattice()
        {
            for (int i = 0; i < this.latticeSize; i++)
                for (int j = 0; j < this.latticeSize; j++)
                    lattice[i, j] = 1;

            return lattice;
        }

        /// <summary>
        /// Custom modulo function because the default % operator does not deal with negative numbers properly.
        /// Calculates x mod m.
        /// </summary>
        /// <param name="dividend"> The number the modulo is taken of (Dividend). </param>
        /// <param name="m"> The number with which the division is done (Divisor). </param>
        /// <returns> The remainder. </returns>
        int Modulo(int dividend, int divisor)
        {
            return (dividend % divisor + divisor) % divisor;
        }

        /// <summary>
        /// Method to write this lattice flattened to a vector to a .csv file.
        /// </summary>
        /// <param name="writer"> The streamwriter to add this lattice to. </param>
        public void PrintLattice(StreamWriter writer)
        {
            int[] flatLattice = new int[latticeSize * latticeSize];

            for (int i = 0; i < latticeSize; i++)
                for (int j = 0; j < latticeSize; j++)
                    flatLattice[i * latticeSize + j] = lattice[i, j];

            // Convert the flattened array to one string, each value seperated by a , .
            writer.WriteLine(String.Join(",", flatLattice.Select(i => i.ToString()).ToArray()));

        }

        /// <summary>
        /// Method to write the temperature of this lattice a .csv file.
        /// </summary>
        /// <param name="writer"> The streamwriter to add this lattices tempearture to. </param>
        public void PrintTemp(StreamWriter writer)
        {
            // Change the culture of the thread to make sure that . is used as the decimal instead of ,    .
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            writer.WriteLine(temperature);
        }
    }
}
