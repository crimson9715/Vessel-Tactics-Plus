hello this is crimson9715, creator of this mod for vessel tactics 0.03a called "vessel tactics+" (very creative name!!!)
this was mainly created for fun to see how units would perform if they had a few small tweaks to make them perform better overall (but also for myself to learn the current systems of the game)
you can see what changes I made in "changelog.txt" (make sure you read this so you know what to expect!!!)
(note that this is not an official product so it may cause bugs / issues with the game, but it should be stable overall)

this was made via extracting the game's assets and code via unity asset ripper, then using dnspy to put edited code back into the game, you can find them here:
https://github.com/AssetRipper/AssetRipper
https://github.com/dnSpy/dnSpy


to install, first install vessel tactics here (assuming that 0.03a is public, or wherever else you got it from): https://meshisoftworks.itch.io/vessel-tactics
then put the contents of the folder named "mod contents" into the top layer of your installation of 0.03a (the .exe file should be on top), make sure that the changed files are properly replaced
there's also an optional folder called "thought edits" if you wish to experience 0-2 and 0-3 with the units that normally stand still to instead chase after you for an extra challenge (along with lamia in sandbox mode being able to chase after units that are out of her range)
easiest way to check if the mod is installed is to go to vessel details and check covenant to see if she has any new attacks

I've also included the source code of the mod (the contents of the Assembly-CSharp.dll file) under "source code", this lets you see the current edited contents of the original file (along with some already-existing code) if you're curious as to what I changed specifically or want to figure out how the code works yourself
there's also a bonus folder called "vanilla lamia addition" that's just the vanilla game, but with a patch installed to allow lamias to be used in vanilla if you don't want the new stuff


mainly looking for feedback, bug reports, or suggestions on what to change for this mod

faq:

q: what folders do I need to use to use the mod?
a: only use "mod contents", the rest are either supplements or cool extra stuff

q: help I can't install the mod!!!
a: you can manually go through the files and replace them yourself, though it shouldn't be very hard to do (mainly depends on how well you know how to use file explorer itself)

q: can you release a version of the game yourself that has the mod pre-installed?
a: personally I'd prefer not to, considering that at most it takes a few minutes to install the mod

q: how do access debug menu in sandbox mode?
a: during a controllable unit's turn, right-click to hide the ui then press "p"; from here, you can manually spawn units and adjust their stats (note that this is a thing you can do in vanilla)

q: why don't the pre-placed units in mission mode / sandbox have any of the additions / stat changes?
a: afaik there's no current way to edit missions yourself, which unfortunately means I cannot fix the units to include them

q: is there any documentation on modding vt?
a: cyrk has made a basic guide for how some of the systems work, along with how to mod in new text (although it's a bit outdated as it was made for 0.02a, along with not explaining everything in proper detail & requiring unity junk, so I wouldn't recommend using it yourself; ask around if you need help in figuring out something)

q: can I use this mod on older / newer versions of vessel tactics?
a: I would highly recommend to not do this as not only is this not supported, but you'll likely end up with an installation that won't work properly or has missing content

q: will you port this to future versions?
a: depending on how complex the game gets in the future code-wise, I'll try to (depending on if I have the time and understanding to do so), though feel free to do so yourself if I can't

q: can I make my own edits of the code and post them online?
a: sure, give credit to me wherever you're sharing it though

q: what can you currently do with the game modding-wise?
a: as the game runs on unity, I can't do much outside of modifying the code present in Assembly-CSharp.dll (as most of the game has assets that can only be modified if you have the original project files yourself, not to mention that I myself am not a dedicated programmer so I only know basic stuff in terms of modding),
but I can edit the basic code of the game like attacks and unit stats; you can check yourself what I have available via using unity asset ripper yourself

q: hello can I repost / copy this somewhere?
a: sure but I ask that you link back to the original post so others know where to go for questions

q: hello I want to use something that's in this mod may I use it?
a: sure, give me credit though (and tell me about it if it's cool!!!)

q: hello I want to give you a suggestion / bug report / feedback / modding questions / talk to you about something else, where can I contact you?
a: you can contact me on discord (tag is "crimson9715.")

q: can I get you do to a vt modding thing for me / implement my suggestion for this mod?
a: depends on if I'm ok with it and I think it's cool

q: is cyrk (the creator of vessel tactics) aware of this mod / cool with it?
a: yeah they're aware of it and think it's cool that something like this exists, though I believe that they're likely going to keep anything mod-related unofficial (aside from adding in features that make it easier for modders)

q: is there an official vessel tactics discord somewhere?
a: yes, but afaik it only seems to be for patrons of the vessel tactics patreon: https://www.patreon.com/MeshiSoftworks/ 

q: can you ask cyrk questions for me / give their tag?
a: I'd rather not as cyrk likely wants to stick to their small dedicated fanbase

q: what does cyrk think is cool?
a: https://www.youtube.com/watch?v=2RBiwJdfoDk

q: favorite units?
a: theano, lamia, and covenant

q: when did you learn about vessel tactics?
a: around 0.01a from a certain dedicated forum (you know the one)

q: I think you are stinky!!!
a: :(

credits:
crimson9715 - main mod creator / coder / custom dialogue 
Cyrk - minor coding assistance (mainly explaining how some of the code works, & the status-on-hit code)
Darling Devil - idea for covenant's radiation ability / inspiration for covenant stat buffs