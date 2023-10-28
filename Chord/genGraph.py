import subprocess
import pandas as pd
import matplotlib.pyplot as plt
import sys

# Define the range of iterations (1 to n, inclusive)
n = int(sys.argv[1])
req=int(sys.argv[2])
interval = int(sys.argv[3])

# Run dotnet run and collect output data
data = []
for i in range(1, n + 1, interval):
    print(f"Running dotnet run {str(i)} {req}")
    result = subprocess.run(["dotnet", "run", str(i), f"{req}"], capture_output=True, text=True)
    data.append(result.stdout)


# Read data from the CSV file
df = pd.read_csv("output.csv")

# Plot a graph of NumNodes vs AverageHops
plt.plot(df["NumNodes"], df["AverageHops"], marker="o")
plt.xlabel("NumNodes")
plt.ylabel("AverageHops")
plt.title("NumNodes vs AverageHops")
plt.grid(True)
# Save the graph as a PNG image
plt.savefig(f"output_graph_{n}_{req}.png")
plt.show()
