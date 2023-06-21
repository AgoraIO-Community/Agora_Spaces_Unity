
# Agora Spaces - building your  own 3D Metaverse Spaces

You want to build a cool meta-verse built for people to join and meet. You are wondering how to enable user engagement with video/voice chat and event synchronization. You are in the right place. This guide walks you through building your own meta-verse application to include features like video chat, spatial audio, media player and other real-time actions.

## Getting Started

The tools we'll use for this projects are:

- **Unity** - a game engine to support 3D object rendering. We recommend Unity Editor version 2021.3 LTS for the best results.

- **Mirror** - a platform kit to support cross-machine character synchronization. There are a few good choices. We use Mirror-Networking SDK in this project after considering the trade-offs.

- **Agora** - the top choice of Voice and Video SDKs for the Unity platform. Sign up for 10,000 free minutes to use every month and obtain an Application ID to use with the SDK.

- **Protobuf** - data serialization library from Google, included for Real-time Transcription feature.
### Dependency
- For JSON Serialization, import NewtonSoft JSON library from the Unity Package Manager.  Installed via Git URL: *com.unity.nuget.newtonsoft-json*.


### How to Run Project
1. Clone this project to a local folder.  The tools will be included in the repo except Agora's SDK.
2. Go to Package Manager in Unity Editor, find, download and import the Agora Video SDK (v4.2.0 when this guide is written)
3. Enter your AppID to the Assets/AgoraSpaces/_AppIDInput/AppIDInput asset object.  If your AppID is created for Test Mode, then you don't need to enter token value.  ![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/Agora_Spaces_Unity_-appid.jpg)
4. Add all the scenes from Assets/AgoraSpaces/Scenes to the Build Settings, with the SpaceOffline scene being the first in the list.
5. Build the project. Make sure that Camera and Microphone usage descriptions are provide if deploying to MacOS or iOS. 
6. Run the App on the target machines.  

[Optional 4.5] Use the icon Assets/AgoraSpaces/Textures/playstore-icon for the default App Icon.

### Login Screen
There are four fields to enter when entering the App:
![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/Agora_space_login.png)

|Field|Description|Example|
|--------|-------|----|
|User Name|Alphanumeric string as user name|MacUser001
|Server|Server's Internet location|localhost, 192.168.2.111
|Room Name|A name that identifies the Agora Channel|agoraspaces
|Network|Your role in the Mirror network|Host or Client

Note, a Host = both a server and client in Mirror's architecture.  If you are running as a Host, then you should provide the IP address to the clients for the Server field.  If you want to host a Mirror server somewhere so everyone can connect as a client, then build this project as a Dedicated Server and deploy to your server environment.  See more information on Mirror's [documentation](https://mirror-networking.gitbook.io/docs/hosting/pragmatic-hosting-guide) page.

## Overview

The AgoraSpaces app will have users log in to a server of their choice and go into a space for exploration and meeting other people.  In the set that uses both Host and Client roles, a host is running the server. Clients can join or leave the space anytime. If the host leaves the space, the session of this space is ended. Users in the space can show or hide themselves from the camera. They also have the option to turn on Real-time transcription to display the conversation in text.

## Folder Structure
```
Assets
├── Agora-RTC-Plugin
├── AgoraSpaces
├── Joystick Pack
├── Mirror
├── Protobuf
├── ScriptTemplates
├── StreamingAssets
```


This table describes the ownership of the above folders
|Folder|Owner|Use|
|--------|-------|----|
|Agora-RTC-Plugin|Agora SDK|Video SDK Plugin|
|AgoraSpaces|AgoraSpaces App|project source code and assets|
|Joystick Pack|Joystick controller asset|movement control library|
|Mirror|Mirror SDK|Object synchronization|
|Protobuf|Protobuf library|data serialization|
|ScriptTemplates|Mirror SDK|script template|
|StreamingAssets|Agora SDK|Supplemental assets|


## Architecture

#### Application and Agora Backend

![Architecture](https://agoracdn.s3.us-west-1.amazonaws.com/images/AgoraSpaces+-+architecture.png)

#### Application Networking - Agora and Mirror Networking

![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/AgoraSpaces+-+App+Arch.png)

## User  Flow Diagram

![User flow diagram](https://agoracdn.s3.us-west-1.amazonaws.com/images/AgoraSpaces+-+Userflow.png)

## App Screens

Login Screen

<div>

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-sign-in.png" width="400" />

</div>

Player Field Screens

<div>

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-playerfield.png" width="400" />

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-playerfield2.png" width="400" />

</div>

Options

<div>

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-options.png" width="400" />

</div>

RTT dialog and language selection

<div>

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-STT-Jap.png" height="240" />

<img  src="https://agoracdn.s3.us-west-1.amazonaws.com/images/AS-lang.png" height="240" />

</div>

## Agora Overview

The main feature of the `AgoraSpaces` app is the ability to have video and voice interaction between users. We will be using Agora for this. Agora is a real-time communication platform that allows you to build video calls, voice calls, and live streaming into your application. And it can handle all the real time communication that will be needed for this application. Furthermore, through the powerful SD-RTN (Software-Defined Real-time Network), Agora provides nice add-on features like Spatial Audio, Media Player and Real-time Transcription.

## Agora Features

On the client side we use the `Agora Unity SDK` to enable the features. In this section, we will describe the each  key features in this application and provide the area of the relevant code for reference. Also, we will cover the networking part of synchronizing local and remote objects.

### Video/Voice Call

Video or voice call is a very important part of the application because this is where the user communication will happen. The Video SDK is a super set of the Voice SDK. If the user just wants to use the voice part of the feature, he/she can turn the camera off from the options menu. To get started with basics, you may refer to the [official get-started guide](https://docs.agora.io/en/video-calling/get-started/get-started-sdk?platform=unity) for some familiarity of the SDK. Make sure check the notes on each supporting platform in the accompanying SDK README file.

The Agora Video SDK can be accessed from [Unity Asset Store](https://assetstore.unity.com/packages/tools/video/agora-video-sdk-for-unity-134502), or our [download page](https://docs.agora.io/en/sdks?platform=unity).

Video/Voice call are basic functionalities of the SDK. When a user joins and leaves the Agora channel, the call session starts and ends, respectively.

#### Reference Areas

Check the code in these methods in **AgoraSpaceController.cs**:

```CSharp
        void JoinChannel(uint uid);
        void LeaveChannel();
        void MuteCamera(bool mute);
        void MuteMic(bool mute);
```


#### Networking

The call hierarchy to JoinChannel is triggered during the network connection through Mirror's **NetworkManager**. When the player object is created on the client, the event *OnCreatePlayer* passes the player's uniquely assigned net id to the *JoinChannel* method in *AgoraSpaceController*. This id becomes the UID that Agora uses to identify the user joining the channel. The following code snippet illustrates the mechanism:
```csharp

        [Client]
        void OnCreatePlayer(CreatePlayerMessage createPlayerMessage)
        {
            AgoraSpaceController.Instance.JoinChannel(createPlayerMessage.name);
        }

        [Server]
        IEnumerator WaitForNetID(NetworkConnectionToClient conn)
        {
            yield return new WaitWhile(() => conn.identity == null);
            conn.Send(new CreatePlayerMessage { name = (uint)conn.identity.netId });
        }
```


### Spatial Audio

On top of a connected channel and working sound system, the Spatial Audio feature gives user immersive audio experience inside the 3D world. That is exactly what a Meta-verse application wants to provide. Agora provides  APIs that enable the calculation of sound volume level, attenuation and other attributes from its powerful backend. The client application  sends the position data to get the dynamic effect while the respective game object moves in the scene.

#### Reference Areas

Check the code in these methods in **AgoraSpaceController.cs**:
```CSharp
void InitSpatialAudioEngine();
int UpdateRemotePosition(uint uid, RemoteVoicePositionInfo posInfo);
int UpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp);
```
Check the entire class *SpacePlayer* in **SpacePlayer.cs** for using the controller methods.

#### Networking

The NetworkBehavior class **SpacePlayer** manages the player object across the network. A remote player's world position is synchronized automatically. The SpacePlayer class reports the change in its transform world position through a delegate *OnPlayerPositionChanged*, that calls the *UpdateRemotePosition* in the AgoraSpaceController class. With this information, the Spatial Audio engine can modify the sound output for that remote player.

### Media Player

The Agora Video engine is capable of playing online or on-device media content to a designated game object in the Unity scene. The Media Player is an optional but interesting feature to add to the Meta-verse application.

#### Reference Area

Media Player main controller is in Monobehavior subclass MediaTV.cs. In this example, the player utilizes a channel to play the online media content. The Play() method is invoked by the OptionMenu UI and calls the playback functionality. See this code snippet:
```csharp
public void Play()
{
    if (MediaPlayer == null) return;
    //We use the mpk to simulate the voice of remote users.
    JoinChannelExWithMPK(AgoraSpaceController.Instance.ChannelID, UidUseInMPK, MediaPlayer.GetId());

    var ret = MediaPlayer.Open(MEDIA_URL, 0);
    Debug.Log("Media Open returns: " + ret);

    // Don't listen to this locally
    MediaPlayer.AdjustPlayoutVolume(0);

    if (this.IsLoop)
    {
        MediaPlayer.SetLoopCount(-1);
    }
    else
    {
        MediaPlayer.SetLoopCount(0);
    }
}
```

Note that since we enable Spatial Audio in this project, the Media Player, represented by a big TV screen in the scene, is also a game object in the 3D space. Therefore, Spatial Audio effect is also enabled for this and included in the MediaTV class. See code in ```SpatialAudioStart``` . We will discuss more in the next section about Spatial Audio.

#### Networking

Only the host can start and stop the media playback through the options menu. The Play button will grey out if a client opens the the  option menu.

### Real-time Transcription

Real-time Transcription is also known as the Speak-to-Text (STT) feature in the code. It is a service on the Agora backend that can be accessed via RESTful APIs from the Unity client. As the time this blog is written, 14 spoken languages are supported. User enables the RTT feature through the Options Menu. As users in the channel speakers, the transcription will be displayed on UI dialog in the scene. Conceptually, the Unity client acts as the receiver for the incoming RTT data stream. On the Agora backend, there are two virtual users set up to process the client's audio data and send data stream to the subscribing client.

#### Reference Area

* STTSupport/STTManager.cs - data processor and logic handler to the RESTFul APIs

* STTSupport/STTModel.cs - data model

* STTSupport/ProtoBuffUtility - low level data parser and handler

* AgoraRTTController.cs - high level controller to utilize STTSupport modules

**Data stream callback**

```csharp
// Receiving data stream text from STT remote service
public override void OnStreamMessage(RtcConnection connection, uint remoteUid, int streamId, byte[] data, uint length, ulong sentTs)
{
    //Debug.Log(String.Format("remoteUid: {0}", remoteUid));
    // if (remoteUid == (int)app.LocalUID + app.STT_Datastream_ID)
    {
        AgoraSTTSample.Protobuf.Text t = ProtobufUtility.ParseProtobufData(data);
        app._rttController.STTTextHandler(t);
    }
}
```

#### Networking

Each user (client or host) decides if he/she wants to use the RTT feature. Special networking logic is not required.

## Other  Key Features

### JoyStick Input

The popular "WASD" keyboard input is available for desktop environment. For the mobile environment, we included the Joystick Pack library. The move and jump controlling logic is provided in the *PlayerController.cs* script.


## Conclusion
This project builds a backbone of an system for a multiuser environment that people would expect to see in a 3D meta-verse space.  It enables important voice and video features defines how people engage in the online world.  Developers can easily use the code to build or enrich their existing project with similar feature.



