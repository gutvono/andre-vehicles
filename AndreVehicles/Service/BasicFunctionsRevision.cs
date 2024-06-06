using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BasicFunctionsRevision
    {
        private BasicFunctionsRevision()
        {
            BasicFunctionsTest();
        }


        public static BasicFunctionsRevision GetInstance()
        {
            return new BasicFunctionsRevision();
        }

        private void BasicFunctionsTest()
        {

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Id: " + i);
            }

            List<string> collection = new List<string>();
            collection.Add("test1");
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }

            string switch_on = "op1";
            switch (switch_on)
            {
                case "op1":
                    Console.WriteLine("OP1");
                    break;
                default:
                    Console.WriteLine("Top Default");
                    break;
            }

            int j = 0;
            while (j < 10)
            {
                Console.WriteLine("Data: " + j);
                j++;
            }

            do
            {
                Console.WriteLine("Data: " + j);
                j++;
            } while (j < 10);
        }
    }
}
