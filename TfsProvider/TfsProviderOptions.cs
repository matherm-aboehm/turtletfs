﻿using System;
using System.Xml.Serialization;

namespace TurtleTfs
{
	[Serializable, XmlRoot(ElementName = "Parameters")]
	public class TfsProviderOptions
	{
		public TfsProviderOptions()
		{
		}

		[XmlElement]
		public string ServerName { get; set; }

		[XmlElement]
		public string UserName { get; set; }
		
		[XmlElement]
		public string UserPassword { get; set; }

		[XmlElement]
		public string ProjectName { get; set; }

		[XmlElement]
		public bool VisualStudioOnline { get; set; }
	}
}