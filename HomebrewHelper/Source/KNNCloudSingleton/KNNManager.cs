using Newtonsoft.Json;

namespace HomebrewHelper.Source.KNNCloudSingleton
{
    public class KNNManager : IManageKNN
    {
        PointData data = new PointData();

        public void AddPoint(Monster point)
        {
            data.points.Add(point);
            if (data.dimensionMaximums.Length == 0) 
            {
                data.dimensionMaximums = new int?[point.Hyperposition.Length];
                point.Hyperposition.CopyTo(data.dimensionMaximums, 0); 
            }
            if (data.dimensionMinimums.Length == 0)
            {
                data.dimensionMinimums = new int?[point.Hyperposition.Length];
                point.Hyperposition.CopyTo(data.dimensionMinimums, 0);
            }

            for (int i = 0; i < point.Hyperposition.Length; i++) 
            {
                if (point.Hyperposition[i] > data.dimensionMaximums[i] || data.dimensionMaximums[i] == null)
                {
                    data.dimensionMaximums[i] = point.Hyperposition[i];
                }
                if (point.Hyperposition[i] < data.dimensionMinimums[i] || data.dimensionMinimums[i] == null)
                {
                    data.dimensionMinimums[i] = point.Hyperposition[i];
                }
            }
        }

        public void Clear() 
        {
            data.points = new List<Monster>();
            data.dimensionMaximums = new int?[0];
            data.dimensionMinimums = new int?[0];
        }

        public int EstimateLevel(int?[] position, int count)
        {
            List<Monster> neighbors = GetNearestNeighbors(position, count);
            return (int)Math.Round(neighbors.Select(e => (double)e.Level).Aggregate((acc, e) => acc + e) / (double)count);
        }

        public List<Monster> GetNearestNeighbors(int?[] position, int count)
        {
            if (data.points.Count == 0) return new List<Monster>();
            if (position.Length != data.dimensionMaximums.Length) return new List<Monster>();

            List<(double, Monster)> neighbors = new List<(double, Monster)>();

            foreach (Monster monster in data.points) 
            {
                double sum = 0;
                bool doNotConsider = false;
                for (int i = 0; i < monster.Hyperposition.Length; i++) 
                {
                    if (position[i] == null)
                    {
                        continue;
                    } 
                    else if (monster.Hyperposition[i] == null) 
                    {
                        doNotConsider = true;
                        break;
                    }
                    
                    sum += Math.Pow(((double)monster.Hyperposition[i] - (double)position[i]) / ((double)data.dimensionMaximums[i] - (double)data.dimensionMinimums[i]), 2) * data.weights[i];
                }

                if (doNotConsider) continue;

                double distance = Math.Sqrt(sum);

                bool placed = false;
                for (int i = 0; i < neighbors.Count; i++) 
                {
                    if (distance < neighbors[i].Item1) 
                    {
                        neighbors.Insert(i, (distance, monster));
                        placed = true;
                        break;
                    }
                }

                if (neighbors.Count < count && !placed)
                {
                    neighbors.Add((distance, monster));
                } 
                else if (neighbors.Count > count) 
                {
                    neighbors = neighbors.GetRange(0, count);
                }
            }

            return neighbors.Select(e => e.Item2).ToList();
        }

        public void SetWeights(double[] weights)
        {
            data.weights = weights;
        }
    }

    public class PointData 
    {
        public List<Monster> points = new List<Monster>();
        public int?[] dimensionMaximums = new int?[0];
        public int?[] dimensionMinimums = new int?[0];

        public double[] weights = new double[0];
    }
}
