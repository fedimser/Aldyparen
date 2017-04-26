using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization;

namespace Aldyparen
{
    [Serializable]
    public class Movie
    {
        private List<Frame> frames;

        public Movie()
        {
            frames = new List<Frame>();
        } 

        
        public Movie(Movie movie)
        {
            frames = new List<Frame>();
            foreach (Frame frame in movie.frames)
            {
                frames.Add(frame.clone());
            }
        }

        public void save(String fileName)
        {
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public static Movie load(String fileName) {
            IFormatter formatter = new  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new FileStream(fileName, 
                                      FileMode.Open, 
                                      FileAccess.Read, 
                                      FileShare.Read);
            Movie obj = (Movie)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        public void appendFrame(Frame frame)
        {
            frames.Add(frame);
        }

        public Frame this[int i]
        {
            get {
                if (i < 0) i = frames.Count + i;
                return frames[i]; 
            }
            //set { InnerList[i] = value; }
        }

        public int frameCount()
        {
            return frames.Count;
        }

        public void removeFrames(int start, int end)
        {
            if (start >= end) return;
            if (start < 0) start = 0;
            if (end > frames.Count) end = frames.Count;

            if (end == start + 1) frames.RemoveAt(start);
            else frames.RemoveRange(start, end - start);
        } 

    }
}
