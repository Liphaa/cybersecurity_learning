 # Integrating AI into a Malware Analysis Sandbox
 In this lab specifically we're going to be working with Large Language Models (LLM) to give an upgrade to our malware analysis lab. In this example we're going to be leveraging off Claudes capabilities to enhance analysis. This lab allows Claude to directly access Ghindra to perform analysis.

 ## What we are starting with
 * A Windows VM with FlareVM installed, this usually comes with java sdk, node.js and Ghindra already so you wont need to download that
* An internet connection, I know previously I told you to set your FlareVM to 'host only' but for this sake set it to 'NAT'
### Setting up dependencies
1. Sign up to Claude with an email and download the desktop version from their site. You can either do free or premium depending on your choice.
2. Download the [newest version of Ghidra MCP](https://github.com/LaurieWired/GhidraMCP/releases)

3. [Download a 'crackme' challenge](https://crackmes.one/) puzzle which are essentially binary samples that are made to be reversed engineered. This is going to be the sample i'm working with today but you can also use samples from [the zoo](https://github.com/ytisf/thezoo) (which are more dangerous so be careful). If you want to recreate the sample I will be working with is called "Tenzo Crackme" and the password to unzip is "crackmes.one"

## Connecting and using the MCP 
1. Navigate and open the "bridge_mcp_ghindra" file in VS code and select "... > Terminal > New Terminal" and install the dependencies using "pip install "mcp[cli]" 
2. Launch Ghidra and go to File > Install Extensions and select the 1.x zip file. Then go to to File > Import File and select your crackme file. Click yes for everything including the prompt to configure Ghindramcp which should pop up.
3. Launch Claude, go to File > Settings > Developer > Edit config and paste this into the "claude_desktop_config.json"
```
"mcpServers": {
    "ghidra": {
      "command": "python",
      "args": [
        "/ABSOLUTE_PATH_TO/bridge_mcp_ghidra.py", // replace this with your path to your bridge_mcp_ghidra.py
        "--ghidra-server",
        "http://127.0.0.1:8080/"
      ]
    }
  }
```
add and extra pair of surrounding curly brackets if there is nothing previously in the file but if there is already then add it as another json object

so my file ended up looking like this since i had a preferences object
```
{
"preferences": {
    "coworkWebSearchEnabled" : true,
    "coworkScheduledTasksEnabled" : false, 
    "ccdScheduledTasksEnabled" : false, 
},

"mcpServers": {
    "ghidra": {
      "command": "python",
      "args": [
        "/ABSOLUTE_PATH_TO/bridge_mcp_ghidra.py", // replace this with your path to your bridge_mcp_ghidra.py
        "--ghidra-server",
        "http://127.0.0.1:8080/"
      ]
    }
  }
}
```
After saving this "ghidra" should show up as a server back on the developers page, however I noticed for me it doesn't unless theres an actually binary sample loaded into Ghidra so if you don't see it try importing a binary sample.

4. Now you are ready to use Claude. Simply give it a prompt and ask about the file "Can you give me a comprehensive analysis of the crack me file loaded in ghidra", it will give you a few prompts to allow capabilities and you can allow or deny them. Have fun!





### Fun thing to do, set up Claude Locally (optional)
For this we will need to download Claude, you can do it the traditional way with downloading from the site and using internet with it but I prefer being able to host it locally so I will be using ollama as a base. Simply go into your powershell and type

```
irm https://ollama.com/install.ps1 | iex
irm https://claude.ai/install.ps1 | iex

ollama launch claude
```

and then you can select your model using your arrow keys, i'm personally going with qwen3.5

To add customisations to Claude we can make something called a Modelfile, this essentially is a file containing the settings of what your agent should be like.

1. Make a modelfile anywhere on your desktop and name it "Modelfile"
2. Edit the configurations
```
FROM qwen3.5:latest
PARAMETER temperature 1 //makes the ai more creative
PARAMETER num_ctx 4096 // allows up to 4096 tokens to be used to generate the context

SYSTEM you are a malware expert, your task is to translate technical findings comprehensively //system message to specify behaviours of chat model
```

then run 

```
ollama create <choose-a-model-name> -f <location of the file e.g. ./Modelfile>
ollama run <choose-a-model-name>
```

congrats you now have your custom model. I didn't end up using a localised version simply because my VM was not able to handle it but if you have a stronger device perhaps you could run it locally.

