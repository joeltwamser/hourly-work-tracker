# Hourly-Work Tracker

The Hourly-Work Tracker is a Windows Desktop App that you can use to track the work you do at your computer.  Simply run it while you are working, pause it during breaks, and reset and store your session when you are done for the day.  It essentially is a stopwatch that calculates, displays, and stores how much money you have made both for your current working session, and over your total use of the app.

To use, open the Configure Window (Figure 2) by right-clicking anywhere on the Tracker Window (Figure 1), and click "Configure".  Enter your Hourly Wage that you earn while working in the "Update Hourly Wage" Textbox, and click the "Save" button.  On the Tracker Window (Figure 1) we are ready to begin tracking our work.  Click "Start" to begin a session.  The top number in the Tracker Window (Figure 1) is the money you have made this current working session, and the lower and smaller number is the money you have made overall while using this app.  Note: You can edit this number (the money you have made overall) on the Tracker Tab in the Configure Window (Figure 2) by entering a new "Total Money Made" value in the lower Textbox on that page.

### Figure 1

![TrackerView](https://user-images.githubusercontent.com/41800319/208805373-0dcacf13-b1cf-4874-a487-eabafbffb536.png)

### Figure 2

![ConfigureWindowTrackerTab](https://user-images.githubusercontent.com/41800319/208808475-5103c0ad-6561-459b-94a6-145e569f74e2.png)

This app is also very visually customizable.  On the "CustomizeUI" Tab of the Configure Window (Figure 3) you will see all sorts of options available to you.  There are 5 Color Pickers allowing you to select the color of the Tracker Window's background, border, buttons, button text, and ticker text.  There is also an opacity slider that allows you to make the Tracker Window more or less transparent, and there is also a very handy feature I love, the "Pop To Front" checkbox.  When this is checked, the Tracker Window will remain the forward-most window on your screen even when it does not have focus.  This is perfect for shrinking the window down, making it slightly transparent, and leaving it up in the corner of your screen to silently tick away while you work, perfect for people who are money-driven and like to have a little visual motivational tool.

### Figure 3

![ConfigureWindowUITab](https://user-images.githubusercontent.com/41800319/208808514-7f7a276d-f96e-4f93-9321-424abb8a7b0f.png)

All of your sessions are stored in a file called "Session logs.csv" in the same directory where your app is kept.  Each Session is comprised of 4 data points, Session Start Date/Time, Session Duration (in # of hours), Hourly Wage of Session, and Wages Earned.  Say you are a freelance developer, webdesigner, video editor etc, this would be a simple but useful tool to record your hours as to when you worked, how long you worked, and how much you charged for a particular job.  You could simply copy this file and send it to whoever you need to invoice.

Also worth mentioning is that the apps memory is written to and read from a text file called "appstate.conf".  I recommend simply not messing with this file or the app might crash and/or behave strangely.  If you are ever having those problems, deleting appstate.conf will reset the app, you'll unfortunately lose your Total Money Earned, but you could re-enter it from the Session logs.

## Future Features?
-Add visual flare like animating the ticker and the dollar signs while running.  
-Create a secure hash for "appstate.conf" so if it is ever modified, the program will know and exit gracefully.  
-Add more Views, including length of session directly only the Tracker Window UI
