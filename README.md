# ArduinoScriptGenerator
Allows a user to simply paste in their HEX file path (absolute) and will give the command line scripts to run in order to flash your Arduino.


This is tested on an Arduino Leonardo. 
The main use case is for flashing the Leonardo with TMK hex files from the HASU Usb Usb keyboard converter project. 

For those who do not know, this allows any keyboard (IBM Model M or any other USB accepting keyboard) to be reprogrammed.

Why? 
It's a fun side project, that gives a reason to code. Primarily in C (QMK/TMK) but also a reason to play around with a low risk C# console app (this app). 

Why else?
I really like having BackSpace where the CAPS lock key is.. I almost never use CAPS, unless I am writing SQL, but even then, it isn't a huge loss to me. 
Using a Leonardo with a USB Host Shield easily allows me to re-program any keyboard to be the same. And this Console App makes it much easier to simply paste a file,
and immediately have the scripts to run to program the Leonardo.
