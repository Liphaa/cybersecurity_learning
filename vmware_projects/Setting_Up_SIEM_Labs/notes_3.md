# Setting up Wazuh SIEM tool and XDR

## Quick Requirements
Before I got started with my lab I had to have a few main things. I will be using Wazuh because it's free and opensourced.
1. An ubuntu server (bridged)
2. A windows vm designed to host the agent

## Downloading Key and Manager
In your Ubuntu server you would wanna download your key and manager. The key is to make sure the files you're downloading is verified. It basically downloads the key file then outputs it to a repository that is used to verify packages. Keys are usually in plaintext so they needed to dearmor it to binary for the 'apt' runner to understand

```
#To Download the Key
curl -s https://packages.wazuh.com/key/GPG-KEY-WAZUH | sudo gpg --dearmor -o
/usr/share/keyrings/wazuh-archive-keyring.gpg

# To download the executable script
curl -sO https://packages.wazuh.com/4.12/wazuh-install.sh && sudo bash
./wazuh-install.sh -a -i

```
At this point you will be given a username and password, PLEASE REMEMBER THEM. I don't need to tell you this. 

Flags present:
* -a means all the components (manager + indexer)
* -i means running in interactive mode
* -s is silent mode so you won't have any progress bars clogging up your screen
* -O means keep original name
* -o is used to specify output location
* --dearmor is used to turn plaintext into binary for the apt to use

## Accessing browser Dashboard
Still on your Ubuntu get your IP address via


```
hostname -I

#OR

ifconfig
```
I like the first one cause it's shorter. Then paste it into your browser "https://<insert_ubuntus_ip>", then log in with the credentials from earlier if you lost it use
```
cat ~/.bash_history
```

## Generating Agent Key
Now go you need to generate an agent key for your host agents, this is like an ID.

```
sudo /var/ossec/bin/manage_agents
```

And fill out the things you ask it. To get IP for windows use "ipconfig" instead of "ifconfig". Extract the key and copy it onto your other VM.

## Running Wazuh Agent in Windows

Now switch to the windows vm and download the agent at: https://documentation.wazuh.com/current/installation-guide/wazuh-agent/index.html

Click the download wizard and follow the steps. In order to have a agent theres only two steps

1. Collect the managers (Ubuntus IP)
2. Collect the extracted key from the manager earlier

Then save, restart and refresh. Now you should see the agent on the ubuntu browser dashboard.


## Troubleshooting

For some reason after installing the Wazuh agent, my windows device would tell me it's running but my ubuntu server doesn't recognise the host. In order to check for version updates I had to look for the ossec file logs. Usually the details about the file versions would be in the last 50 lines of the code so i inspected that

```
sudo tail -n 50 /var/ossec/logs/ ossec.log
```

From that I had discovered that my manager had a different version (4.12.0) then my agent (4.14.4-1) so i needed to update my manager with

```
sudo apt install wazuh-manager=4.14.4-1
```

In which after that it worked fine.

## Adding File Integrity Checks
This is pretty easy, all you need to do is edit the configuration

1. Run Notepad as Administrator
2. Open the ossec.conf file in "C:\Program Files (x86)\ossec-agent\ossec.conf"
3. Paste the directory you want to keep an eye on in the conf file 

```<directories realtime="yes">C:\Insert\path\here</directories>```

I did this next to another line that kinda looks like this to keep things neat and it's good practice.

Now if you make any changes (edit/delete) to any files in the path you gave it will appear in your browser dashboard!