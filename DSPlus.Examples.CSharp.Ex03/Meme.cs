namespace DSPlus.Examples
{
    public class Meme
    {
        public string Name;
        public string Path;
        public string Prefix;

        public Meme(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public Meme(string name, string path, string prefix)
        {
            Name = name;
            Path = path;
            Prefix = prefix;
        }

    }
}
