using System.ComponentModel.Design;

namespace OnlineMusicStore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MusicStore.GetMenu();
        }
    }
}
