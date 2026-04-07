# How To Set Up a Virtual Machine
Setting up a virtual machine is relatively easy, all it takes is installation of VMWare and an ISO Image file. For this example I am going to be using Windows ISO but you can use a Kali Linux (for penetration testing) or Ubuntu Linux (for server hosting).

- [Vmware Install Link](https://www.vmware.com/)
- [An ISO image of Windows 10/11 (for me i'm going to be using Windows 10)](https://www.microsoft.com/en-us/software-download/windows10)
- A relatively strong Desktop, should be around 4GB+ in RAM, and 64GB+ in Storage/HardDisk/SSDs. But thats just minimum requirements, if you want your VM (Virtual Machine) to not lag or crash I would recommend investing 8GB+ in RAM and 256GB+ in Storage

<ol>
  <li>Go to 'File' > 'New Virtual Machine'</li>
  <li>Select 'Typical' > 'Installer Disk Image File' and select your downloaded ISO image</li>
  <li>Name your virtual machine and select where you want it to be (keep this default if you don't know what you are doing)</li>
  <li>Select the Disk Size, and either "Store Virtual Disk as a Single File" or "Split into multiple files" it doesn't really matter unless you're moving vms to different computers</li>
  <li>Click on Customize Hardware, this is up to personal preference but here are my configurations for running a smooth experience</li>

Memory: 8GB,
Processors: 2 (1 core per processor),
Hard Disk: 80GB,
Network Adapter: Host-only (this is my personal preference for setting up windows machine you'll see why later),
Keep all other settings defaulted
<li>Click 'Finish' and now you've created your first virtual machine</li>
<li>Follow through the installation process, everything now should be in plain English so if in doubt just select the default on everything</li>
</ol>

## Why did I set to host-only?
Notice earlier to how I set the network mode to "host-only"? Well I needed to do this in order to run a new windows environment without it asking for a microsoft account. Ideally if I were to be testing malware and stuff for it I would not want my microsoft credentials to be on the device or go through the hassle of making a new one that's why I choose to launch it without internet. I learnt this through trial and error.

If you want to go back to the internet I suggest going to VM > Settings > Network Adapter > NAT or Bridged. What's the difference? There is a youtube video to explain it in depth but to put in plainly 'NAT' sends internet traffic through your computers IP address so all outbound traffic would look like it's coming from your host computer while 'Bridged' established the virtual machine as its own 'computer' with its own IP address. Bridged is BAD for malware analysis because a virus can potentially reach all other devices on your network while NAT is just bound to your host computer's private internal networks (so other VMs).

## Troubleshooting
### Creating new network
Say you wanna make a separate network for isolating vms
1. Go to Edit > Virtual Network Editor > Change Settings (accept admin privileges) > Add network
2. Specify whether you want a host-only (no internet internal network), NAT or Bridged (explain above)
3. Specify the ip address range you want them to be (optional)
4. Go the virtual machine you want allocated for that network (make sure its suspended or off) VM > Settings > Network Adapters > Select Custom > Select the network you just made 'VNetX'
5. For malware analysis only use host-only, and only switch to NAT when you need to download files such as malware samples
### Copy and Paste Not Working
1. You need your vm paused/ shut off for this (the button is on the top left menu, you always need it to be suspended for most settings)
2. Go to VM > Settings > Options > Guest Isolation > Enable Drag and Drop + Enable copy and paste
3. Run your vm
4. Go to VM, Click Install VMware tools and a prompt should be going up for installation, just click accept

### Internet not working anymore
I ran into this problem when I was switching from host-only to NAT, most likely culprit is DNS not configured on the device. DNS is basically the server that gives your ip human-readable names like google.com instead of 8.8.8.8 usually it's your home router or sits on your local area network (LAN).

1. On your host device (your desktop not the vm) go to type "Command Prompt" in your search bar and type "ipconfig /all" and copy the IP address next to DNS server.
2. Go into your vm go into command prompt again, select "Run as Administrator" and type these
```
netsh interface show interface // this will give you your interface name
netsh interface ip set dns name="<add interface name here>" static <add dns ip address here> //do not type <add interface here directly>

//what my command looked like
netsh interface ip set dns name="Ethernet0" static 8.8.8.8
```




