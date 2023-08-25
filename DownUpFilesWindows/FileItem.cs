namespace DownUpFilesWindows
{
    public class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public int _rawSize = 0;
        public int RawSize { get => _rawSize; set => _rawSize = value; }
        public bool IsDirectory { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is FileItem)
            {
                var e = obj as FileItem;
                return e.GetHashCode() == this.GetHashCode();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int c = 0, x = 0;
            foreach( var ch in Name)
            {
                c += ch;
                x += ch;
            }
            int c1 = 0;
            foreach( var ch in Path)
            {
                c1 += ch;
                x += ch;
            }
            c &= c1;
            foreach( var ch in Size)
            {
                c += ch;
                x += ch;
            }
            c1 |= c;
            c1 <<= (IsDirectory? 1 : 0);
            c1 += x;
            c1 += _rawSize;
            return c1;
        }
    }
}
