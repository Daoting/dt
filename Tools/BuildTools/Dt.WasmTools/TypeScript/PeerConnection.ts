namespace Dt
{
    export class PeerConnection
    {
        connection: RTCPeerConnection

        private constructor(private element: HTMLElement, private isCaller: boolean, private localVideoId: string, private remoteVideoId: string)
        {
            this.connection = new RTCPeerConnection();
            this.AttachEvent();
        }

        public async SetAnswer(spdAnswer: RTCSessionDescriptionInit)
        {
            try
            {
                await this.connection.setRemoteDescription(spdAnswer);
            } catch (err)
            {
                console.error(this.getPrefix() + "setRemoteDescription 时异常：" + err);
            }
        }

        public async AddIceCandidate(candidate: RTCIceCandidate)
        {
            try
            {
                await this.connection.addIceCandidate(candidate);
            } catch (err)
            {
                console.error(this.getPrefix() + "addIceCandidate 时异常：" + err);
            }
        }

        public Close(): void
        {
            if (!this.connection)
                return;

            this.connection.onicecandidate =
                this.connection.oniceconnectionstatechange =
                this.connection.onsignalingstatechange =
                this.connection.onnegotiationneeded =
                this.connection.ontrack = () => { };

            var video = this.localVideo();
            if (video && video.srcObject)
            {
                video.pause();
                (video.srcObject as MediaStream).getTracks().forEach(track => track.stop());
            }

            video = this.remoteVideo();
            if (video && video.srcObject)
            {
                video.pause();
                (video.srcObject as MediaStream).getTracks().forEach(track => track.stop());
            }

            this.connection.close();
            this.connection = null;
        }

        public static async CreateCaller(element: HTMLElement, localVideoId: string, remoteVideoId: string): Promise<PeerConnection>
        {
            const peer = new PeerConnection(element, true, localVideoId, remoteVideoId);
            if (await peer.AddMediaTrack())
                return peer;

            peer.Close();
            return null;
        }

        public static async CreateReceiver(element: HTMLElement, iceOffer: RTCSessionDescriptionInit, localVideoId: string, remoteVideoId: string): Promise<PeerConnection>
        {
            const peer = new PeerConnection(element, false, localVideoId, remoteVideoId);
            await peer.connection.setRemoteDescription(iceOffer);

            if (await peer.AddMediaTrack())
            {
                const answer = await peer.connection.createAnswer();
                await peer.connection.setLocalDescription(answer);
                peer.raiseEvent("Answer", peer.connection.localDescription);
                console.log("【Recver】Create Answer");
                return peer;
            }
            else
            {
                peer.Close();
                return null;
            }
        }

        //#region 事件
        AttachEvent()
        {
            // 开始/重新进行ICE谈判事件，只在发起方触发
            this.connection.onnegotiationneeded = async (ev) =>
            {
                console.log(this.getPrefix() + "Negotiation needed");
                if (this.connection.signalingState === "stable")
                {
                    // 创建offer
                    const offer = await this.connection.createOffer({ offerToReceiveVideo: true, offerToReceiveAudio: false });
                    await this.connection.setLocalDescription(offer);

                    // 向信令服务发送offer
                    this.raiseEvent("Offer", this.connection.localDescription);
                }
            };

            // 交换ICE候选事件
            this.connection.onicecandidate = ev =>
            {
                if (ev.candidate && ev.candidate.candidate)
                {
                    console.log(this.getPrefix() + "Ice Candidate");
                    // 通过信令服务发送给对方
                    this.raiseEvent("IceCandidate", ev.candidate);
                }
            };

            // 向连接中添加磁道事件
            this.connection.ontrack = ev =>
            {
                console.log(this.getPrefix() + "Receive Track");
                const video = this.remoteVideo()
                video.oncanplay = ev =>
                {
                    // 开始播放事件，此时视频宽高已确定，只处理一次
                    var div = document.getElementById(this.remoteVideoId);
                    var height = (div.clientWidth / video.clientWidth) * video.clientHeight;

                    video.setAttribute('width', div.clientWidth.toString());
                    video.setAttribute('height', height.toString());
                    video.oncanplay = null;
                };
                // 对方addTrack时的stream
                video.srcObject = ev.streams[0];
                // 外部可用来开始计时
                this.raiseEvent("Track");
            };

            // ICE连接状态更改事件
            this.connection.oniceconnectionstatechange = ev =>
            {
                console.log(this.getPrefix() + "ICE connection state：" + this.connection.connectionState);
                switch (this.connection.connectionState)
                {
                    case "closed":
                    case "failed":
                    case "disconnected":
                        this.raiseEvent("Closed");
                        break;
                }
            };

            // 信令进程的状态更改事件
            this.connection.onsignalingstatechange = ev =>
            {
                console.log(this.getPrefix() + "signaling state：" + this.connection.signalingState);
                switch (this.connection.signalingState)
                {
                    case "closed":
                        this.raiseEvent("Closed");
                        break;
                }
            };

            //this.connection.onicegatheringstatechange = ev =>
            //{
            //    console.log(this.getPrefix() + "ICE gathering state：" + this.connection.iceGatheringState);
            //};
        }

        raiseEvent(eventName: string, detail: any | undefined = undefined)
        {
            if (detail)
            {
                const eventToManagedCode = new CustomEvent(eventName, { detail: detail });
                this.element.dispatchEvent(eventToManagedCode);
            } else
            {
                const eventToManagedCode = new Event(eventName);
                this.element.dispatchEvent(eventToManagedCode);
            }
        }
        //#endregion

        //#region 内部方法
        async AddMediaTrack(): Promise<boolean>
        {
            try
            {
                const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                const video = this.localVideo();
                video.oncanplay = ev =>
                {
                    // 开始播放事件，此时视频宽高已确定，只处理一次
                    var div = document.getElementById(this.localVideoId);
                    var height = (div.clientWidth / video.clientWidth) * video.clientHeight;

                    video.setAttribute('width', div.clientWidth.toString());
                    video.setAttribute('height', height.toString());
                    video.oncanplay = null;
                };
                video.srcObject = stream;
                // 添加一组媒体轨道传输给对方，stream是这一组轨道的同步流
                stream.getTracks().forEach(track => this.connection.addTrack(track, stream));
                return true;
            }
            catch (err)
            {
                this.raiseEvent("DeviceError", err);
            }
            return false;
        }

        getPrefix(): string
        {
            return this.isCaller ? "【Caller】" : "【Recver】";
        }

        localVideo(): HTMLMediaElement
        {
            return document.getElementById(this.isCaller ? "callerLocalVideo" : "recverLocalVideo") as HTMLMediaElement;
        }

        remoteVideo(): HTMLMediaElement
        {
            return document.getElementById(this.isCaller ? "callerRemoteVideo" : "recverRemoteVideo") as HTMLMediaElement;
        }
        //#endregion
    }
}