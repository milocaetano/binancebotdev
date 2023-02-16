namespace BinanceImporter.App
{
    public interface ICsvExtrator
    {
        List<T> GetObjects<T>(string file);
    }
}