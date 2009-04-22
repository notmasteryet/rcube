using System;
using System.IO;
using System.Xml.Serialization;

namespace RCube
{
    public class CubeStorage
    {
        static XmlSerializer serializer = new XmlSerializer(typeof(CubeStorage));

        public int[] Colors;
        public int[][] SideColors;
        public double[] Rotation;

        public static CubeStorage Load(string filename)
        {            
            using (FileStream fs = File.OpenRead(filename))
            {
                return (CubeStorage)serializer.Deserialize(fs);
            }
        }

        public void Save(string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                serializer.Serialize(fs, this);
            }
        }
    }

    
}
