// Quick program to build and test RegionMap class.  
// Built and debuged in VS2010

namespace POC_RegionMap
{
	class Program
	{
		static void Main(string[] args)
		{
			// Read the static parts of the map.
			Map map = new Map();

			map.ReadStatic();

			RegionMap.Setup(map);
		}
	}
}
