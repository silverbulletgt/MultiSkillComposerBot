This project was designed to demonstrate an issue which occurs when a composer dialog calls more than 1 skill.

## Setting up the project
### Bot.Skills
This project needs to be running on local IIS
There is a publish profile in the project which publishes the project to: C:\inetpub\wwwroot\Bot.MultiSkill.Skills
Open MultiSkillAdaptiveBot.sln in Visual Studio
Right click on Bot.Skills & choose "Publish"
Publish using "FolderProfile.pubxml"
Open IIS
Create a new application pool called "Bot.MultiSkill.Skills" which is set to "No Managed Code"
Open Sites / Default Site & right click on "Bot.MultiSkill.Skills" & "Convert To Application" selecting the "Bot.MultiSkill.Skills" application pool.

This can be tested as working by going to be below 2 URLs in a browser:
http://localhost/bot.multiskill.skills/api/manifest/Test.Skill1
http://localhost/bot.multiskill.skills/api/manifest/Test.Skill2

You should see JSON text as the response similar to the below:
{"$schema":"https://schemas.botframework.com/schemas/skills/skill-manifest-2.1.preview-0.json","$id":"Test.Skill2","name":"Test.Skill2","description":"Responds with a message indicating Test Skill 2 was successfully called.","publisherName":"Test","version":"1.0","iconUrl":null,"tags":[],"endpoints":[{"name":"production","protocol":"BotFrameworkV3","description":"Production endpoint","endpointUrl":"http://localhost/Bot.MultiSkill.Skills/api/skill/Test.Skill2","msAppId":"00000000-0000-0000-0000-000000000000"}]}

###ComposerMultiSkillDialog

This project was built using [Bot Framework Composer](https://docs.microsoft.com/en-us/composer/introduction)
Open the project in Bot Framework Composer.
Click "Start Bot"
When that action is completed select your preference of "Open Web Chat" or "Test in Emulator"

### The Issue
When using the intent "CallBoth" with the text "Call Both" the expected conversation output is:

- "In Call Both"
- "Response From Test Skill 1."
- "Response From Test Skill 2."
- "Finished Call Both"

Though the actual output is:
- "In Call Both"
- "Response From Test Skill 1."

For some reason the dialog/intent "CallBoth" is ending when the 1st skill is called.

After that if "Basic Intent" is sent then the response is:
- "In Basic Intent" (which is expected)
- "Response From Test Skill 2"
- "Finished Call Both"

The 2nd skill & the ending "Finished Call Both" are then presented after "In Basic Intent".

It appears that the call to the 2nd skill is somehow getting stuck & only appearing after another action occurs.

#### What I have tried
I currently think that the reason this is occurring is because the return await stepContext.EndDialogAsync(result); in DoActionDialogBase.cs (line 40)
Is impacting the calling dialog (CallBoth) & causing it to end as well.

I originally had EndConversation instead of EndDialog so changed that but it didn't have any impact.

I also thought that perhaps the 2 skills are interferring with each other & since 1 is finished the other cannot start.
I tried to change the name of the property created in conversation state in SkillBotBase.cs (line 33) from "DialogState" to $"DialogState_{Dialog.Id}" to make it unique.
I thought that maybe the conversation state was already in an "ended" state which was preventing the 2nd skill from starting.
That did not have any impact though.

I don't know what else to try at this point.

## Project Details
This repo contains 2 components:
- Bot.Skills (in IIS Bot.MultiSkill.Skills)
- ComposerMultiSkillDialog

### Bot.Skills (in IIS Bot.MultiSkill.Skills)
This project is a Web API which exposes the skill manifest & skill endpoint which are used by ComposerMultiSkillDialog

It contains 2 skills: TestSkill1 & TestSkill2

These are in the files: Bot.Skills/Dialogs/TestSkill1Dialog & Bot.Skills/Dialogs/TestSkill2Dialog

The manifests for these skills is generated through the ManifestController.  
That controller uses reflection to lookup all of the bots which are available in the project.
It does this by looking for classes which inherit from SkillManifestBase.
When SkillManifestBase is implemented the "RelatedDialog" type is set.

To get the manifest for each skill use the below URLs:
http://localhost/bot.multiskill.skills/api/manifest/Test.Skill1
http://localhost/bot.multiskill.skills/api/manifest/Test.Skill2

A skill is called through the SkillConsumerController.
This controller also uses reflection to lookup the details for the {skillName} which is passed as part of the URL.

The URL for calling the skills will be:
http://localhost/Bot.MultiSkill.Skills/api/skill/Test.Skill1
http://localhost/Bot.MultiSkill.Skills/api/skill/Test.Skill2

### ComposerMultiSkillDialog

This project was created using the Bot Composer [Bot Framework Composer](https://docs.microsoft.com/en-us/composer/introduction).

It has 4 Intents:
- TestSkill1: called using the text "Call Skill 1"
- TestSkill2: called using the text "Call Skill 2"
- Basic Intent: called using the text "Basic Intent"
- CallBoth: called using the text "Call Both"
    - This calls both skills - This is where the issue exists