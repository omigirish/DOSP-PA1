import pandas as pd
import matplotlib.pyplot as plt

# Read data from the CSV file
df = pd.read_csv("output.csv")
gossip = df.query("Algorithm == 'Gossip'")
Push_Sum = df.query("Algorithm == 'Push Sum'")


for topology in ['Line','2D_Grid', 'Imperfect_3D_Grid', 'Full_Network']:
    x = gossip.query(f"Topology=='{topology}'")["Nodes"]
    y = gossip.query(f"Topology=='{topology}'")["ConvergenceTime"]
    plt.plot(x, y, label = topology) 
plt.legend() 
plt.xlabel('Num of Nodes')
plt.ylabel('Convergence Time(s)')
plt.title("Convergence Time vs Num Nodes for Gossip Algorithm")
plt.figure(figsize=(10,50))
plt.show()
