# Agora Spaces: Building Your Own 3D Metaverse Spaces

You want to build a cool metaverse in which people can join and meet. If you are wondering how to enable user engagement with video and voice chat and event synchronization you’re in the right place. This guide walks you through building your own metaverse application to include video chat, spatial audio, media player, and other real-time actions.

## Getting Started

The tools we'll use for this project:

- **Unity**: A game engine to support 3D object rendering. We recommend Unity Editor version 2021.3 LTS for the best results.

- **Mirror**: A platform kit to support cross-machine character synchronization. A few good choices are available. We use the Mirror Networking SDK in this project after considering the trade-offs.

- **Agora**: The top choice of Voice and Video SDKs for the Unity platform. Sign up for 10,000 free minutes to use every month and obtain an Application ID to use with the SDK.

- **Protobuf**; A data serialization library from Google, included for the real-time transcription feature.
### Dependency
- For JSON serialization, import the NewtonSoft JSON library from the Unity Package Manager, which is installed via this Git URL: *com.unity.nuget.newtonsoft-json*.


### To Run the Project
1. Clone this project to a local folder. The tools are included in the repo, except for the Agora SDK.
2. Go to the Package Manager in Unity Editor, and find, download, and import the Agora Video SDK (v4.2.0 at the time of writing).
3. Enter your App ID in the Assets/AgoraSpaces/_AppIDInput/AppIDInput asset object. If your App ID is created for test mode, then you don't need to enter a token value. ![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/Agora_Spaces_Unity_-appid.jpg)
4. Add all the scenes from Assets/AgoraSpaces/Scenes to the build settings, with the SpaceOffline scene first in the list.
5. Build the project. Make sure that camera and microphone usage descriptions are provided if deploying to macOS or iOS. 
6. Run the app on the target machines.  

[Optional 4.5] Use the icon Assets/AgoraSpaces/Textures/playstore-icon for the default app icon.

### Login Screen
You have four fields to complete when entering the app:
![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/Agora_space_login.png)

|Field|Description|Example|
|--------|-------|----|
|User Name|Alphanumeric string as user name|MacUser001
|Server|Server's internet location|localhost, 192.168.2.111
|Room Name|A name that identifies the Agora channel|agoraspaces
|Network|Your role in the Mirror network|host or client

Note: A host is both a server and client in the Mirror architecture. If you are running as a host, then you should provide the IP address to the clients for the Server field. If you want to host a Mirror server somewhere so that everyone can connect as a client, then build this project as a dedicated server and deploy it to your server environment. For more information, see the Mirror [documentation](https://mirror-networking.gitbook.io/docs/hosting/pragmatic-hosting-guide) page.

## Overview

The AgoraSpaces app will have users log in to a server of their choice and go into a space for exploration and meeting other people. In the set that uses both host and client roles, a host is running the server. Clients can join or leave the space anytime. If the host leaves the space, the session of this space is ended. Users in the space can show or hide themselves from the camera. They also have the option to turn on real-time transcription to display the conversation in text.

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


This table describes the ownership of the above folders:
|Folder|Owner|Use|
|--------|-------|----|
|Agora-RTC-Plugin|Agora SDK|Video SDK Plugin|
|AgoraSpaces|AgoraSpaces app|Project source code and assets|
|Joystick Pack|Joystick controller asset|Movement control library|
|Mirror|Mirror SDK|Object synchronization|
|Protobuf|Protobuf library|Data serialization|
|ScriptTemplates|Mirror SDK|Script template|
|StreamingAssets|Agora SDK|Supplemental assets|


## Architecture

#### Application and Agora Backend

![Architecture](https://agoracdn.s3.us-west-1.amazonaws.com/images/AgoraSpaces+-+architecture.png)

#### Application Networking: Agora and Mirror Networking

![enter image description here](https://agoracdn.s3.us-west-1.amazonaws.com/images/AgoraSpaces+-+App+Arch.png)

## User Flow Diagram

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

The main feature of the `AgoraSpaces` app is the ability to have video and voice interaction between users. For this, we use the Agora real-time communication platform, which enables you to build video calls, voice calls, and live streaming into your application. And it can handle all the real-time communication needed for this application. Furthermore, through the powerful Software-Defined Real-Time Network (SD-RTN), Agora provides add-on features like spatial audio, media player, and real-time transcription.

## Agora Features

On the client side we use the `Agora Unity SDK` to enable the features. In this section, we describe each of the key features in this application and provide the area of the relevant code for reference. Also, we cover the networking part of synchronizing local and remote objects.

### Video and Voice Calls

Video and voice calls are a very important part of the application because this is where the user communication happens. The Video SDK is a superset of the Voice SDK. If the user wants to use only the voice part of the feature, they can turn the camera off from the Options menu. To get started with the basics, see the [official get-started guide](https://docs.agora.io/en/video-calling/get-started/get-started-sdk?platform=unity) to familiarize yourself with the SDK. Make sure to check the notes on each supporting platform in the accompanying SDK README file.

The Agora Video SDK can be accessed from [Unity Asset Store](https://assetstore.unity.com/packages/tools/video/agora-video-sdk-for-unity-134502), or our [download page](https://docs.agora.io/en/sdks?platform=unity).

Video and voice calls  are basic functionalities of the SDK. When a user joins or leaves the Agora channel, the call session starts or ends, as appropriate.

#### Reference Areas

Check the code in these methods in **AgoraSpaceController.cs**:

```CSharp
        void JoinChannel(uint uid);
        void LeaveChannel();
        void MuteCamera(bool mute);
        void MuteMic(bool mute);
```


#### Networking

The call hierarchy to JoinChannel is triggered during the network connection through Mirror's **NetworkManager**. When the player object is created on the client, the event *OnCreatePlayer* passes the player's uniquely assigned Net ID to the *JoinChannel* method in *AgoraSpaceController*. This ID becomes the UID that Agora uses to identify the user joining the channel. The following code snippet illustrates the mechanism:
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

On top of a connected channel and working sound system, the spatial audio feature gives users an immersive audio experience in the 3D world. That is exactly what a metaverse application is meant to provide. Agora APIs enable the calculation of sound volume level, attenuation, and other attributes from its powerful backend. The client application  sends the position data to get the dynamic effect while the respective game object moves in the scene.

#### Reference Areas

Check the code in these methods in **AgoraSpaceController.cs**:
```CSharp
void InitSpatialAudioEngine();
int UpdateRemotePosition(uint uid, RemoteVoicePositionInfo posInfo);
int UpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp);
```
Check the entire class *SpacePlayer* in **SpacePlayer.cs** for using the controller methods.

#### Networking

The NetworkBehavior class **SpacePlayer** manages the player object across the network. A remote player's world position is synchronized automatically. The SpacePlayer class reports the change in its transform world position through a delegate *OnPlayerPositionChanged*, which calls the *UpdateRemotePosition* in the AgoraSpaceController class. With this information, the Spatial Audio engine can modify the sound output for that remote player.

### Media Player

The Agora Video engine is capable of playing online or on-device media content to a designated game object in the Unity scene. The media player is an optional but interesting feature to add to the metaverse application.

#### Reference Area

The media player main controller is in the Monobehavior subclass MediaTV.cs. In this example, the player uses a channel to play the online media content. The Play() method is invoked by the OptionMenu UI and calls the playback functionality. See this code snippet:
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

Since we enable spatial audio in this project, the media player, represented by a big TV screen in the scene, is also a game object in the 3D space. Therefore, the spatial audio effect is also enabled for this and included in the MediaTV class. See the code in ```SpatialAudioStart``` . We discuss spatial audio more in the next section.

#### Networking

Only the host can start and stop the media playback through the Options menu. The Play button is grayed out when a client opens the Options menu.

### Real-Time Transcription

Real-time transcription is also known as the speak-to-text (STT) feature in the code. It is a service on the Agora backend that can be accessed via RESTful APIs from the Unity client. At the time of writing, 14 spoken languages are supported. Users enable the RTT feature through the Options menu. As users in the channel speak, the transcription is displayed in the UI dialog box in the scene. Conceptually, the Unity client acts as the receiver for the incoming RTT data stream. On the Agora backend, two virtual users are set up to process the client's audio data and to send a data stream to the subscribing client.

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

Each user (client or host) decides if they want to use the RTT feature. Special networking logic is not required.

## Other  Key Features

### JoyStick Input

The popular WASD keyboard input is available for the desktop environment. For the mobile environment, we included the Joystick Pack library. The move and jump controlling logic is provided in the *PlayerController.cs* script.


## Conclusion
This project builds a backbone of a system for a multiuser environment that people would expect to see in a 3D metaverse space. It enables important voice and video features and defines how people engage in the online world. Developers can easily use the code to build or enrich their existing project with similar features.



