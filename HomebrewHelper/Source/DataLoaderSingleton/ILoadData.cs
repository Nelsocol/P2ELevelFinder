namespace HomebrewHelper.Source.DataLoaderSingleton
{
    public interface ILoadData
    {
        public Task LoadData();
        public Task<List<Monster>> LoadTestData();
    }
}
