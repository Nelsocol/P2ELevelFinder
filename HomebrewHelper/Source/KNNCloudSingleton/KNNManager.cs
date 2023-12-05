namespace HomebrewHelper.Source.KNNCloudSingleton
{
    public class KNNManager : IManageKNN
    {
        private List<Monster> points = new List<Monster>();
        private int?[] dimensionMaximums = new int?[0];
        private int?[] dimensionMinimums = new int?[0];

        private double[] weights = new double[0];

        public void AddPoint(Monster point)
        {
            points.Add(point);
            if (dimensionMaximums.Length == 0) 
            {
                dimensionMaximums = new int?[point.Hyperposition.Length];
                point.Hyperposition.CopyTo(dimensionMaximums, 0); 
            }
            if (dimensionMinimums.Length == 0)
            {
                dimensionMinimums = new int?[point.Hyperposition.Length];
                point.Hyperposition.CopyTo(dimensionMinimums, 0);
            }

            for (int i = 0; i < point.Hyperposition.Length; i++) 
            {
                if (point.Hyperposition[i] > dimensionMaximums[i] || dimensionMaximums[i] == null)
                {
                    dimensionMaximums[i] = point.Hyperposition[i];
                }
                if (point.Hyperposition[i] < dimensionMinimums[i] || dimensionMinimums[i] == null)
                {
                    dimensionMinimums[i] = point.Hyperposition[i];
                }
            }
        }

        public void Clear() 
        {
            points = new List<Monster>();
            dimensionMaximums = new int?[0];
            dimensionMinimums = new int?[0];
        }

        public int EstimateLevel(int?[] position, int count)
        {
            List<Monster> neighbors = GetNearestNeighbors(position, count);
            return (int)Math.Round(neighbors.Select(e => (double)e.Level).Aggregate((acc, e) => acc + e) / (double)count);
        }

        public List<Monster> GetNearestNeighbors(int?[] position, int count)
        {
            if (points.Count == 0) return new List<Monster>();
            if (position.Length != dimensionMaximums.Length) return new List<Monster>();

            List<(double, Monster)> neighbors = new List<(double, Monster)>();

            foreach (Monster monster in points) 
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
                    
                    sum += Math.Pow(((double)monster.Hyperposition[i] - (double)position[i]) / ((double)dimensionMaximums[i] - (double)dimensionMinimums[i]), 2) * weights[i];
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
            this.weights = weights;
        }
    }
}
