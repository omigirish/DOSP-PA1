import subprocess
import pandas as pd
import matplotlib.pyplot as plt
import sys

# Run dotnet run and collect output data
numNodes = [10000, 25000, 50000, 75000, 100000]
algorithms = ['gossip', 'pushsum']
topologies= ['full', '2D', 'imp3D']
for n in numNodes:
    for algorithm in algorithms:
        for top in topologies:
            print(f"dotnet run {n} {top} {algorithm}")
            subprocess.run(["dotnet", "run", str(n), top, algorithm], text=True)
            

