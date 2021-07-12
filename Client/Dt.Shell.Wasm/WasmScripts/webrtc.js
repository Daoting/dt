var Dt;
(function (Dt) {
    class PeerConnection {
        constructor(element) {
            this.element = element;
            this.connection = new RTCPeerConnection();
            this.AttachEvent();
        }
        AttachEvent() {
            this.connection.onicecandidate = ev => {
                if (ev.candidate && ev.candidate.candidate)
                    this.raiseEvent("IceCandidate", ev.candidate);
            };
            this.connection.oniceconnectionstatechange = ev => {
                console.log("ICE connection state：" + this.connection.connectionState);
                switch (this.connection.connectionState) {
                    case "closed":
                    case "failed":
                    case "disconnected":
                        this.raiseEvent("Closed");
                        break;
                }
            };
            this.connection.onicegatheringstatechange = ev => {
                console.log("ICE gathering state：" + this.connection.iceGatheringState);
            };
            this.connection.onsignalingstatechange = ev => {
                console.log("signaling state：" + this.connection.signalingState);
                switch (this.connection.signalingState) {
                    case "closed":
                        this.raiseEvent("Closed");
                        break;
                }
            };
            this.connection.onnegotiationneeded = ev => {
                console.log("Negotiation needed：" + this.connection.signalingState);
            };
            this.connection.ontrack = ev => {
                document.getElementById("received_video").srcObject = ev.streams[0];
                this.raiseEvent("Track");
            };
        }
        async AddMediaTrack() {
            try {
                const webcamStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
                document.getElementById("local_video").srcObject = webcamStream;
                webcamStream.getTracks().forEach(track => this.connection.addTransceiver(track, { streams: [webcamStream] }));
                return true;
            }
            catch (err) {
                this.raiseEvent("DeviceError", err);
            }
            return false;
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
        async SetAnswer(spdAnswer) {
            try {
                await this.connection.setRemoteDescription(spdAnswer);
            }
            catch (err) {
                console.error("setRemoteDescription 时异常：", err);
            }
        }
        async AddIceCandidate(candidate) {
            try {
                await this.connection.addIceCandidate(new RTCIceCandidate({ candidate: candidate }));
            }
            catch (err) {
                console.error("addIceCandidate 时异常：", err);
            }
        }
        Close() {
            if (!this.connection)
                return;
            this.connection.onicecandidate =
                this.connection.oniceconnectionstatechange =
                    this.connection.onicegatheringstatechange =
                        this.connection.onsignalingstatechange =
                            this.connection.onnegotiationneeded =
                                this.connection.ontrack = () => { };
            this.connection.getTransceivers().forEach(transceiver => transceiver.stop());
            const localVideo = document.getElementById("local_video");
            if (localVideo.srcObject) {
                localVideo.pause();
                localVideo.srcObject.getTracks().forEach(track => track.stop());
            }
            this.connection.close();
            this.connection = null;
        }
        static async CreateCaller(element) {
            const peer = new PeerConnection(element);
            if (peer.AddMediaTrack()) {
                const offer = await peer.connection.createOffer({ offerToReceiveVideo: true, offerToReceiveAudio: false });
                await peer.connection.setLocalDescription(offer);
                peer.raiseEvent("Offer", peer.connection.localDescription);
                return peer;
            }
            else {
                peer.Close();
                return null;
            }
        }
        static async CreateReceiver(element, iceOffer) {
            const peer = new PeerConnection(element);
            if (peer.AddMediaTrack()) {
                await peer.connection.setRemoteDescription(iceOffer);
                const answer = await peer.connection.createAnswer();
                await peer.connection.setLocalDescription(answer);
                peer.raiseEvent("Answer", peer.connection.localDescription);
                return peer;
            }
            else {
                peer.Close();
                return null;
            }
        }
    }
    Dt.PeerConnection = PeerConnection;
})(Dt || (Dt = {}));
//# sourceMappingURL=webrtc.js.map