using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace ConnectionTester
{
	class Program
	{
		public static bool useSimple = false;
		static void Main(string[] args)
		{
			if(useSimple)
				SimpleLogger.Run(args);
			else
				new ClientTester(args).Run();
		}
    }
}
