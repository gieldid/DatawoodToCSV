﻿using DatawoodGH.Properties;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace DatawoodGH
{
	public class DatawoodInfo : GH_AssemblyInfo
	{
		public override string Name => "Datawood";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => Resources.datwoodcsv.ToBitmap();

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "Extensions for the datawood team in grasshopper";

		public override Guid Id => new Guid("8C789F61-C234-460E-8BC6-A0C92980486B");

		//Return a string identifying you or your company.
		public override string AuthorName => "Giel Jurriëns";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "gieldid@gmail.com";
	}
}