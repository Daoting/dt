var Dt;
(function (Dt) {
    class PeerConnection {
        constructor(element, isCaller, localVideoId, remoteVideoId) {
            this.element = element;
            this.isCaller = isCaller;
            this.localVideoId = localVideoId;
            this.remoteVideoId = remoteVideoId;
            this.connection = new RTCPeerConnection();
            this.AttachEvent();
        }
        async SetAnswer(spdAnswer) {
            try {
                await this.connection.setRemoteDescription(spdAnswer);
            }
            catch (err) {
                console.error(this.getPrefix() + "setRemoteDescription 时异常：" + err);
            }
        }
        async AddIceCandidate(candidate) {
            try {
                await this.connection.addIceCandidate(candidate);
            }
            catch (err) {
                console.error(this.getPrefix() + "addIceCandidate 时异常：" + err);
            }
        }
        Close() {
            if (!this.connection)
                return;
            this.connection.onicecandidate =
                this.connection.oniceconnectionstatechange =
                    this.connection.onsignalingstatechange =
                        this.connection.onnegotiationneeded =
                            this.connection.ontrack = () => { };
            var video = this.localVideo();
            if (video && video.srcObject) {
                video.pause();
                video.srcObject.getTracks().forEach(track => track.stop());
            }
            video = this.remoteVideo();
            if (video && video.srcObject) {
                video.pause();
                video.srcObject.getTracks().forEach(track => track.stop());
            }
            this.connection.close();
            this.connection = null;
        }
        static async CreateCaller(element, localVideoId, remoteVideoId) {
            const peer = new PeerConnection(element, true, localVideoId, remoteVideoId);
            if (await peer.AddMediaTrack())
                return peer;
            peer.Close();
            return null;
        }
        static async CreateReceiver(element, iceOffer, localVideoId, remoteVideoId) {
            const peer = new PeerConnection(element, false, localVideoId, remoteVideoId);
            await peer.connection.setRemoteDescription(iceOffer);
            if (await peer.AddMediaTrack()) {
                const answer = await peer.connection.createAnswer();
                await peer.connection.setLocalDescription(answer);
                peer.raiseEvent("Answer", peer.connection.localDescription);
                console.log("【Recver】Create Answer");
                return peer;
            }
            else {
                peer.Close();
                return null;
            }
        }
        AttachEvent() {
            this.connection.onnegotiationneeded = async (ev) => {
                console.log(this.getPrefix() + "Negotiation needed");
                if (this.connection.signalingState === "stable") {
                    const offer = await this.connection.createOffer({ offerToReceiveVideo: true, offerToReceiveAudio: false });
                    await this.connection.setLocalDescription(offer);
                    this.raiseEvent("Offer", this.connection.localDescription);
                }
            };
            this.connection.onicecandidate = ev => {
                if (ev.candidate && ev.candidate.candidate) {
                    console.log(this.getPrefix() + "Ice Candidate");
                    this.raiseEvent("IceCandidate", ev.candidate);
                }
            };
            this.connection.ontrack = ev => {
                console.log(this.getPrefix() + "Receive Track");
                const video = this.remoteVideo();
                video.oncanplay = ev => {
                    var div = document.getElementById(this.remoteVideoId);
                    var height = (div.clientWidth / video.clientWidth) * video.clientHeight;
                    video.setAttribute('width', div.clientWidth.toString());
                    video.setAttribute('height', height.toString());
                    video.oncanplay = null;
                };
                video.srcObject = ev.streams[0];
                this.raiseEvent("Track");
            };
            this.connection.oniceconnectionstatechange = ev => {
                console.log(this.getPrefix() + "ICE connection state：" + this.connection.connectionState);
                switch (this.connection.connectionState) {
                    case "closed":
                    case "failed":
                    case "disconnected":
                        this.raiseEvent("Closed");
                        break;
                }
            };
            this.connection.onsignalingstatechange = ev => {
                console.log(this.getPrefix() + "signaling state：" + this.connection.signalingState);
                switch (this.connection.signalingState) {
                    case "closed":
                        this.raiseEvent("Closed");
                        break;
                }
            };
        }
        raiseEvent(eventName, detail = undefined) {
            if (detail) {
                const eventToManagedCode = new CustomEvent(eventName, { detail: detail });
                this.element.dispatchEvent(eventToManagedCode);
            }
            else {
                const eventToManagedCode = new Event(eventName);
                this.element.dispatchEvent(eventToManagedCode);
            }
        }
        async AddMediaTrack() {
            try {
                const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                const video = this.localVideo();
                video.oncanplay = ev => {
                    var div = document.getElementById(this.localVideoId);
                    var height = (div.clientWidth / video.clientWidth) * video.clientHeight;
                    video.setAttribute('width', div.clientWidth.toString());
                    video.setAttribute('height', height.toString());
                    video.oncanplay = null;
                };
                video.srcObject = stream;
                stream.getTracks().forEach(track => this.connection.addTrack(track, stream));
                return true;
            }
            catch (err) {
                this.raiseEvent("DeviceError", err);
            }
            return false;
        }
        getPrefix() {
            return this.isCaller ? "【Caller】" : "【Recver】";
        }
        localVideo() {
            return document.getElementById(this.isCaller ? "callerLocalVideo" : "recverLocalVideo");
        }
        remoteVideo() {
            return document.getElementById(this.isCaller ? "callerRemoteVideo" : "recverRemoteVideo");
        }
    }
    Dt.PeerConnection = PeerConnection;
})(Dt || (Dt = {}));
//# sourceMappingURL=webrtc.js.map