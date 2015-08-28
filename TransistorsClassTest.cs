/*
 * Created by SharpDevelop.
 * User: Администратор
 * Date: 11.12.2014
 * Time: 21:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace eulerMake
{
	[TestFixture]
	public class TransistorsClassTest
	{
		[Test]
		public void TestMethod()
		{
			// TODO: Add your test.
			Transistors trs = new Transistors();
			trs.setNode("nd1");
			trs.addTrans("tr1", "MBREAKN_NORMAL");
			Dictionary<string, TrUnit> dic1 = trs.getListN();
			Assert.AreEqual(7, dic1.Count);
		}
	}
}
