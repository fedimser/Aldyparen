using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization;

namespace Aldyparen
{
    [Serializable]
    class Movie
    {
        public Frame[] frames;
        


        public Movie(Frame[] _frames, int count)
        {
            frames = new Frame[count];
            for (int i = 0; i < count; i++)
                frames[i] = _frames[i];
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
    }
}
