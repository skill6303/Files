﻿using System;
using System.Threading;
using OpenQA.Selenium.Interactions;

namespace Files.InteractionTests.Tests
{
	[TestClass]
	public class SettingsTests
	{

		[TestCleanup]
		public void Cleanup()
		{
			var action = new Actions(SessionManager.Session);
			action.SendKeys(OpenQA.Selenium.Keys.Escape).Build().Perform();
		}

		[TestMethod]
		public void VerifySettingsAreAccessible()
		{
			TestHelper.InvokeButtonById("SettingsButton");
			AxeHelper.AssertNoAccessibilityErrors();

			var settingsItems = new string[]
			{
				"SettingsItemAppearance",
				"SettingsItemPreferences",
				"SettingsItemFolders",
				"SettingsItemMultitasking",
				"SettingsItemExperimental",
				"SettingsItemAbout"
			};

			foreach (var item in settingsItems)
			{
				for (int i = 0; i < 5; i++)
				{
					try
					{
						Console.WriteLine("Inoking button:" + item);
						Thread.Sleep(2000);
						TestHelper.InvokeButtonById(item);
						i = 1000;
					}
					catch (Exception exc)
					{
						Console.WriteLine("Failed to invoke the button:" + item + " with exception" + exc.Message);
					}

				}
				try
				{
					// First run can be flaky due to external components
					AxeHelper.AssertNoAccessibilityErrors();
				}
				catch (System.Exception) { }
				AxeHelper.AssertNoAccessibilityErrors();
			}
		}
	}
}
