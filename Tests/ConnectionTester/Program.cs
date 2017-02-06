using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace ConnectionTester
{
	class Program
	{
		public static bool testClient = false;

		public static bool useSimple = true;
		static void Main(string[] args)
		{
			if (testClient)
			{
				if(useSimple)
					SimpleLogger.Run(args);
				else
					new ClientTester(args).Run();
			}
			else
			{
				if(useSimple)
					SimpleHoster.Run(args);
			}
		}
    }
}
