using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageResizer;

namespace ThreadedImageResizer
{
    class Program
    {
        static string pathOriginal = @"C:\temp\Images\OriginalImages";
        static string pathZoom = @"C:\temp\Images\ProcessedImages\Zoom";
        static string pathThumbnail = @"C:\temp\Images\ProcessedImages\Thumbnail";
        static string pathStandard = @"C:\temp\Images\ProcessedImages\Standard";

        static void Main(string[] args)
        {
            var originalFiles = Directory.EnumerateFiles(pathOriginal);
            var sw = Stopwatch.StartNew();

            //Base scenario. Single thread on everything.
            foreach (var item in originalFiles)
            {
                ProcessImageResizeSingleThread(item);
            }
            sw.Stop();
            Console.WriteLine("Total time {0}", sw.ElapsedMilliseconds);


            ////Call process image on multiple threads.
            //sw.Restart();
            //originalFiles
            //    .AsParallel()
            //    .ForAll(f => ProcessImageResizeSingleThread(f));
            //sw.Stop();
            //Console.WriteLine("Total time {0}", sw.ElapsedMilliseconds);

            ////Call process image on a single thread but process image will call each image resize on a separate thread.
            //sw.Restart();
            //foreach (var item in originalFiles)
            //{
            //    ProcessImageResizeMultiThread(item);
            //}
            //sw.Stop();
            //Console.WriteLine("Total time {0}", sw.ElapsedMilliseconds);


            
            Console.ReadLine();
        }


       
        public static void ProcessImageResizeSingleThread(string file)
        {
            Console.WriteLine("Process Image SingleThread Id is {0}", Thread.CurrentThread.ManagedThreadId);

            FileInfo fi = new FileInfo(file);

            Thumbnail(fi);
            Zoom(fi);
            Standard(fi);

        }

        public static void ProcessImageResizeMultiThread(string file)
        {
            Console.WriteLine("Process Image MultiThread Id is {0}", Thread.CurrentThread.ManagedThreadId);

            FileInfo fi = new FileInfo(file);

            Task[] tasks = new Task[3];

            tasks[0] = Task.Factory.StartNew(() => Thumbnail(fi));
            tasks[1] = Task.Factory.StartNew(() => Zoom(fi));
            tasks[2] = Task.Factory.StartNew(() => Standard(fi));

            Task.WaitAll(tasks);

        }




        private static void Zoom(FileInfo fi)
        {
            Console.WriteLine("Zoom Thread Id is {0}", Thread.CurrentThread.ManagedThreadId);

            string imgDestination = string.Empty;
            string imgSource = fi.FullName;


            imgDestination = Path.Combine(pathZoom, fi.Name);
            if (!File.Exists(imgDestination))
            {
                Instructions i = new Instructions();
                i.Width = 1000;
                var zoom = new ImageJob(imgSource, imgDestination, i);
                zoom.Build();
            }
        }

        private static void Standard(FileInfo fi)
        {
            Console.WriteLine("Standard Thread Id is {0}", Thread.CurrentThread.ManagedThreadId);

            string imgDestination = string.Empty;
            string imgSource = fi.FullName;

            imgDestination = Path.Combine(pathStandard, fi.Name);
            if (!File.Exists(imgDestination))
            {
                Instructions i = new Instructions();
                i.Width = 500;
                var standard = new ImageJob(imgSource, imgDestination, i);
                standard.Build();
            }
        }

        private static void Thumbnail(FileInfo fi)
        {
            Console.WriteLine("Thumbnail Thread Id is {0}", Thread.CurrentThread.ManagedThreadId);
            string imgDestination = string.Empty;
            string imgSource = fi.FullName;

            imgDestination = Path.Combine(pathThumbnail, fi.Name);
            if (!File.Exists(imgDestination))
            {
                Instructions i = new Instructions();
                i.Width = 260;
                var thumbnail = new ImageJob(imgSource, imgDestination, i);
                thumbnail.Build();
            }
        }

    }
}
