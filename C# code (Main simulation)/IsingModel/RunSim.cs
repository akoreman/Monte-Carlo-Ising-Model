namespace IsingSimulation
{
    /// <note>
    /// Naming convention:
    /// 
    /// variables & parameters: camelCase.
    /// methods & classes: PascalCase.   
    /// single use tokens/indices: single letter undercase char (i,j).
    /// </note>

    /// <summary>
    /// Class to input the parameters and run the simulation.
    /// </summary>
    class RunSim
    {
        /// <summary>
        /// Main entry point of the progam, input the parameters at which you want to run the simulation.
        /// 
        /// Options for flipMethod are "Metropolis" and "Heatbath".
        /// </summary>
        static void Main()
        {
            int latticeSize = 32;
         
            string flipMethod = "Metropolis";

            int timeStepSize = latticeSize * latticeSize;
            int numberEquilibrationTimeSteps = 30; 
            int numberSampleTimeSteps = 10000;

            int numberBootstrapSamples = 10;

            double temperatureRangeStart = 0.1;
            double temperatureRangeEnd = 4.5;
            double temperatureInterval = 0.01;

            MonteCarloSimulation simulationRun = new MonteCarloSimulation(latticeSize, temperatureRangeStart, temperatureRangeEnd, temperatureInterval, flipMethod, numberSampleTimeSteps, numberEquilibrationTimeSteps, timeStepSize, numberBootstrapSamples);
            simulationRun.Run();
        }
    }
}
