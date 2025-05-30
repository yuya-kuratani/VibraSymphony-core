# VibraSymphony-core
<img src="https://img.shields.io/badge/-Unity-000000.svg?logo=unity&style=for-the-badge">

------

## ■ プロジェクト概要
制作時期 : 2024年 2月 ~ 3月、2025年　8月 ~ 10月

**Vibra Symphony**は、**Unity**を用いて**チーム**で開発したプロジェクトです。

* **VRライブ**に音楽と同期した**振動**を組み合わせて、VRでの体験を**よりリアル**なものにするというコンセプトのプロジェクトです。

* ライブ演出を見る**VR側**と実際に振動を起こす**スマートフォン (Android) 側**の2つのアプリを連携させて使用します。

> [!IMPORTANT]
> このリポジトリは**VR (Oculus Quest) 側**のプログラムになります。
> 
> このリポジトリは、**[VibraSymphony-moble リポジトリ](https://github.com/yuya-kuratani/VibraSymphony-mobile)と連携して**利用するものとなっています。

&nbsp;  

## ■ 主な機能
* **スマートフォン側**のローカルストレージ内から好きな音楽ファイルを選択し、TCP/IPで**VR側**に送信します。
* 振動技術
  * **VR側**でベースやドラムなどの**振動を感じる部分**を検知し、OSC通信により**スマートフォン側**で実際に振動を発生させます。
* 映像技術
  * ライブ会場を**360°見回す**ことができ、コントローラーのスティックで**自由に歩き回る**ことや、コントローラーを振ることで**ペンライトを振る**こともできます。
  * 振動のタイミングに同期した**エフェクトや演出**も発生します。

&nbsp;  

## ■ 使用方法
1. **スマートフォン側**で `Main` ボタンを押します。
4. **VR側**の `Devices` を `+` ボタンを押して追加し、 `ID` 欄に**スマートフォン側**で画面左下に表示された `ID` を入力します。
5. **VR側**で `GO!!` ボタンを押します。
6. **スマートフォン側**で `Pick Music` ボタンを押して、好きな音楽ファイルを選択します。
7. 曲の送受信の後、自動的にVRライブが開始します。

&nbsp;  

## ■ デモ
実際にVRで見ることのできるライブ会場の外見です。キャラクター (Unity-Chan) 以外は全て自作しています。

<img width="70%" alt="vibra-symphony_demo_img_1" src="https://github.com/user-attachments/assets/0b8fdcaa-2547-4ba0-8cb1-ec8017e70269" />

<img width="70%" alt="vibra-symphony_demo_img_2" src="https://github.com/user-attachments/assets/40f3769d-116b-45e8-94dc-ed563d94b505" />

<img width="70%" alt="vibra-symphony_demo_img_3" src="https://github.com/user-attachments/assets/2cf596c6-0ca1-4e4b-9ddd-13f4f6402b02" />

<img width="70%" alt="vibra-symphony_demo_img_4" src="https://github.com/user-attachments/assets/0b636d83-6018-4d5b-8e3b-66fedaf7d93c" />

https://github.com/user-attachments/assets/8a085b71-98d2-47ec-9856-923974187763



&nbsp;  

## ■ 使用技術
#### ライブ会場の3Dモデルおよびそのテクスチャ
* Blender

#### 火花のエフェクト
* VFX Graph (Unity)
<img width="50%" alt="vibra-symphony_spark" src="https://github.com/user-attachments/assets/740bb677-981d-494b-9193-48a1064c9c7f" />

#### ステージのディスプレイ
* Shader Graph (Unity)
<img width="50%" alt="vibra-symphony_display" src="https://github.com/user-attachments/assets/83336270-d47f-4be9-bbc7-01f0b6d7b571" />

#### ステージのライト
* Shader Graph (Unity)

頂点シェーダーにより簡単にライトの**太さ**と**長さ**を調整できます。

<img width="50%" alt="vibra-symphony_light" src="https://github.com/user-attachments/assets/e5538930-ddbb-4c3b-945a-21baa16ff2dc" />

#### 会場上部のホログラム
* Shader Graph (Unity)
<img width="50%" alt="vibra-symphony_hologram" src="https://github.com/user-attachments/assets/f7e69844-201c-4936-941e-05f7d4a12663" />

#### スピーカーのエフェクト
* VFX Graph (Unity)

振動のタイミングと同時にエフェクトが発生します。

<img width="50%" alt="vibra-symphony_speaker" src="https://github.com/user-attachments/assets/f98f67f7-3573-45a7-b343-24ea7b6d6682" />

#### ステージ両脇の円形ディスプレイの演出
* Shader Graph (Unity)

振動のタイミングと同時に波が振動します。

<img width="50%" alt="vibra-symphony_wave" src="https://github.com/user-attachments/assets/739a3e86-ad10-4da7-b5bd-3a6c7978abef" />

#### 観客のペンライト
* Compute Shading (Unity)
* Shader Graph (Unity)

Compute Shadingを用いた並列処理によって、大量のペンライトの描画を軽量化することに成功しています。

<img width="50%" alt="vibra-symphony_audience" src="https://github.com/user-attachments/assets/67e03463-e067-4b2a-99e4-bbbc195f4355" />


&nbsp;  

## ■ 注意事項
> [!IMPORTANT]
> 自分が作成したものは、**UIの画像**と**ライブ会場の3Dモデル**、**エフェクトや演出**、そして**ポストプロセス**などのフロントエンド部分であり、それ以外の、システムのプログラムやプロジェクト構成などはチームメンバーの制作物です。

&nbsp;  

## ■ ライセンスおよびクレジット
* 営利目的での利用を禁止します。
* 改変や再配布を禁止します。

------

本プロジェクトでは、ユニティ・テクノロジーズ・ジャパンが提供する「Unity-Chan」キャラクターおよび関連アセットを、以下のライセンスに従って使用しています。
* [Unity-Chan ライセンス（日本語）](https://unity-chan.com/contents/license_jp/)  
* [Unity-Chan License (English)](https://unity-chan.com/contents/license_en/)

©️2025 蔵谷友哉　

©️ Unity Technologies Japan / Unity-Chan Project. All rights reserved.
