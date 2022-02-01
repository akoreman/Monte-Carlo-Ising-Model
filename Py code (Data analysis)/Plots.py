# -*- coding: utf-8 -*-
import matplotlib.pyplot as plt
import InputOutput as IO

"""
Read the files generated by the C# code to plot them in python. 
The number denotes the lattice size the observables were calculated at.
"""
temperatureLabels = IO.ReadCSV('temperatureLabels.csv')

specificHeat8 = IO.ReadCSV('data\SpecificHeat8.csv')
magnetisation8 = IO.ReadCSV('data\Magnetisation8.csv')
susceptibility8 = IO.ReadCSV('data\Susceptibility8.csv')

specificHeat16 = IO.ReadCSV('data\SpecificHeat16.csv')
magnetisation16 = IO.ReadCSV('data\Magnetisation16.csv')
susceptibility16 = IO.ReadCSV('data\Susceptibility16.csv')

specificHeat32 = IO.ReadCSV('data\SpecificHeat32.csv')
magnetisation32 = IO.ReadCSV('data\Magnetisation32.csv')
susceptibility32 = IO.ReadCSV('data\Susceptibility32.csv')

specificHeat128 = IO.ReadCSV('data\SpecificHeat128.csv')
magnetisation128 = IO.ReadCSV('data\Magnetisation128.csv')
susceptibility128 = IO.ReadCSV('data\Susceptibility128.csv')

"""
Reformating the data to comply with pyplot.
"""
specificHeatExpectation8 = [x[0] for x in specificHeat8]
specificHeatError8 = [x[1] for x in specificHeat8]

magnetisationExpectation8 = [x[0] for x in magnetisation8]
magnetisationError8 = [x[1] for x in magnetisation8]

susceptibilityExpectation8 = [x[0] for x in susceptibility8]
susceptibilityError8 = [x[1] for x in susceptibility8]

specificHeatExpectation16 = [x[0] for x in specificHeat16]
specificHeatError16 = [x[1] for x in specificHeat16]

magnetisationExpectation16 = [x[0] for x in magnetisation16]
magnetisationError16 = [x[1] for x in magnetisation16]

susceptibilityExpectation16 = [x[0] for x in susceptibility16]
susceptibilityError16 = [x[1] for x in susceptibility16]

specificHeatExpectation32 = [x[0] for x in specificHeat32]
specificHeatError32 = [x[1] for x in specificHeat32]

magnetisationExpectation32 = [x[0] for x in magnetisation32]
magnetisationError32 = [x[1] for x in magnetisation32]

susceptibilityExpectation32 = [x[0] for x in susceptibility32]
susceptibilityError32 = [x[1] for x in susceptibility32]

specificHeatExpectation128 = [x[0] for x in specificHeat128]
specificHeatError128 = [x[1] for x in specificHeat128]

magnetisationExpectation128 = [x[0] for x in magnetisation128]
magnetisationError128 = [x[1] for x in magnetisation128]

susceptibilityExpectation128 = [x[0] for x in susceptibility128]
susceptibilityError128 = [x[1] for x in susceptibility128]

"""
Plot and save the plots.
"""
plt.errorbar(temperatureLabels, specificHeatExpectation8, specificHeatError8, fmt = 'bo', markersize = 1.5)
plt.errorbar(temperatureLabels, specificHeatExpectation16, specificHeatError16, fmt = 'go', markersize = 1.5)
#plt.errorbar(temperatureLabels, specificHeatExpectation32, specificHeatError32, fmt = 'ro', markersize = 1.5)
plt.errorbar(temperatureLabels, specificHeatExpectation128, specificHeatError128, fmt = 'ro', markersize = 1.5)
plt.xlabel('T [dimensionless units]')
plt.ylabel(r'$c$ [dimensionless units]')
plt.legend(['8x8','16x16','128x128'], loc='upper right')
plt.savefig("SpecificHeatPerSpin.pdf")
plt.show()

plt.errorbar(temperatureLabels, magnetisationExpectation8, magnetisationError8, fmt = 'bo', markersize = 1.5)
plt.errorbar(temperatureLabels, magnetisationExpectation16, magnetisationError16, fmt = 'go', markersize = 1.5)
#plt.errorbar(temperatureLabels, magnetisationExpectation32, magnetisationError32, fmt = 'ro', markersize = 1.5)
plt.errorbar(temperatureLabels, magnetisationExpectation128, magnetisationError128, fmt = 'ro', markersize = 1.5)
plt.xlabel('T [dimensionless units]')
plt.ylabel(r'$\langle m \rangle$ [dimensionless units]')
plt.legend(['8x8','16x16','128x128'], loc='upper right')
plt.savefig("MagnetisationPerSpin.pdf")
plt.show()

plt.errorbar(temperatureLabels, susceptibilityExpectation8, susceptibilityError8, fmt = 'bo', markersize = 1.5)
plt.errorbar(temperatureLabels, susceptibilityExpectation16, susceptibilityError16, fmt = 'go', markersize = 1.5)
#plt.errorbar(temperatureLabels, susceptibilityExpectation32, susceptibilityError32, fmt = 'ro', markersize = 1.5)
plt.errorbar(temperatureLabels, susceptibilityExpectation128, susceptibilityError128, fmt = 'ro', markersize = 1.5)
plt.xlabel('T [dimensionless units]')
plt.ylabel(r'$\chi$ [dimensionless units]')
plt.legend(['8x8','16x16','128x128'], loc='upper right')
plt.savefig("SusceptibilityPerSpin.pdf")
plt.show()