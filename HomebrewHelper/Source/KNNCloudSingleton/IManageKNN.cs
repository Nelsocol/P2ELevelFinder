namespace HomebrewHelper.Source.KNNCloudSingleton
{
    public interface IManageKNN
    {
        public void AddPoint(Monster point);
        public void Clear();
        public List<Monster> GetNearestNeighbors(int?[] position, int count);
        public int EstimateLevel(int?[] position, int count);
        public void SetWeights(double[] weights);
    }
}
