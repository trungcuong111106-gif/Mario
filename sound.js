// ============================================================
// SOUND ENGINE - Ai Là Tỉ Phú
// Web Audio API - không cần file MP3
// ============================================================
const SoundEngine = (() => {
    let ctx = null;
    let enabled = localStorage.getItem('sound_enabled') !== 'false';
    let tickNode = null;
    let bgmGain = null;

    function getCtx() {
        if (!ctx) ctx = new (window.AudioContext || window.webkitAudioContext)();
        return ctx;
    }
    function resume() {
        const c = getCtx();
        if (c.state === 'suspended') c.resume();
        return c;
    }

    // Tạo oscillator ngắn
    function tone(freq, type, dur, vol, delay=0) {
        if (!enabled) return;
        const c = resume();
        const osc = c.createOscillator();
        const g = c.createGain();
        osc.connect(g); g.connect(c.destination);
        osc.type = type;
        osc.frequency.setValueAtTime(freq, c.currentTime + delay);
        g.gain.setValueAtTime(vol, c.currentTime + delay);
        g.gain.exponentialRampToValueAtTime(0.001, c.currentTime + delay + dur);
        osc.start(c.currentTime + delay);
        osc.stop(c.currentTime + delay + dur + 0.01);
    }

    // Tạo noise (dùng cho hiệu ứng)
    function noise(dur, vol, delay=0) {
        if (!enabled) return;
        const c = resume();
        const buf = c.createBuffer(1, c.sampleRate*dur, c.sampleRate);
        const d = buf.getChannelData(0);
        for(let i=0;i<d.length;i++) d[i]=(Math.random()*2-1);
        const src = c.createBufferSource();
        src.buffer = buf;
        const g = c.createGain();
        src.connect(g); g.connect(c.destination);
        g.gain.setValueAtTime(vol, c.currentTime+delay);
        g.gain.exponentialRampToValueAtTime(0.001, c.currentTime+delay+dur);
        src.start(c.currentTime+delay);
    }

    // ===========================
    // NHẠC NỀN (Beat engine)
    // ===========================
    function startBGM(intensity='low') {
        if (!enabled) return;
        stopBGM();
        const c = resume();
        bgmGain = c.createGain();
        bgmGain.gain.value = 0.06;
        bgmGain.connect(c.destination);

        const bpm = intensity==='high'?145 : intensity==='mid'?115 : 88;
        const iv = 60/bpm;
        let beat = 0;

        function scheduleBeat() {
            if (!bgmGain) return;
            const now = c.currentTime;

            // Bass kick
            const kick = c.createOscillator();
            const kg = c.createGain();
            kick.connect(kg); kg.connect(bgmGain);
            kick.frequency.setValueAtTime(110, now);
            kick.frequency.exponentialRampToValueAtTime(28, now+0.12);
            kg.gain.setValueAtTime(1, now);
            kg.gain.exponentialRampToValueAtTime(0.001, now+0.18);
            kick.start(now); kick.stop(now+0.2);

            // Hi-hat nhịp lẻ
            if (beat%2===1) {
                const hh = c.createOscillator();
                const hhg = c.createGain();
                hh.connect(hhg); hhg.connect(bgmGain);
                hh.type='square'; hh.frequency.value=900;
                hhg.gain.setValueAtTime(0.25, now);
                hhg.gain.exponentialRampToValueAtTime(0.001, now+0.04);
                hh.start(now); hh.stop(now+0.05);
            }

            // Melody tension mỗi 4 beat
            if (beat%4===0) {
                const scales = {
                    low:  [110,138,165,185,220],
                    mid:  [146,185,220,246,293],
                    high: [196,247,294,330,392]
                };
                const notes = scales[intensity];
                const f = notes[Math.floor(Math.random()*notes.length)];
                const mel = c.createOscillator();
                const mg = c.createGain();
                mel.connect(mg); mg.connect(bgmGain);
                mel.type='sine'; mel.frequency.value=f;
                mg.gain.setValueAtTime(0.45, now);
                mg.gain.exponentialRampToValueAtTime(0.001, now+iv*0.85);
                mel.start(now); mel.stop(now+iv);
            }

            beat++;
            tickNode = setTimeout(scheduleBeat, iv*1000);
        }
        scheduleBeat();
    }

    function stopBGM() {
        if (tickNode) { clearTimeout(tickNode); tickNode=null; }
        if (bgmGain) {
            try { bgmGain.gain.exponentialRampToValueAtTime(0.001, getCtx().currentTime+0.3); } catch(e){}
            setTimeout(()=>{ if(bgmGain){bgmGain.disconnect();bgmGain=null;} },350);
        }
    }

    // ===========================
    // SỰ KIỆN GAME
    // ===========================

    // Câu hỏi mới
    function playNewQuestion(num) {
        if (!enabled) return;
        stopBGM();
        if (num===5||num===10||num===15) {
            // Mốc an toàn - fanfare nhỏ
            tone(659,'sine',0.15,0.45);
            tone(784,'sine',0.15,0.4,0.14);
            tone(988,'sine',0.25,0.45,0.28);
        } else {
            tone(440,'sine',0.1,0.25);
            tone(554,'sine',0.12,0.2,0.1);
        }
        const intensity = num<=5?'low':num<=10?'mid':'high';
        setTimeout(()=>startBGM(intensity), 350);
    }

    // Chọn đáp án
    function playSelect() {
        tone(500,'sine',0.1,0.3);
        tone(630,'sine',0.08,0.2,0.08);
    }

    // Đáp án ĐÚNG
    function playCorrect() {
        stopBGM();
        const up=[523,659,784,1047];
        up.forEach((f,i)=>tone(f,'sine',0.18,0.45,i*0.1));
        [523,659,784].forEach(f=>tone(f,'sine',0.45,0.3,0.55));
        noise(0.08,0.04,0.45);
    }

    // Đáp án SAI
    function playWrong() {
        stopBGM();
        tone(350,'sawtooth',0.18,0.55);
        tone(280,'sawtooth',0.18,0.5,0.15);
        tone(220,'sawtooth',0.3,0.45,0.32);
        noise(0.12,0.07,0.15);
    }

    // THẮNG 🏆
    function playWin() {
        stopBGM();
        // Giai điệu thắng
        const m=[523,523,523,392,523,659,784];
        const t=[0,0.16,0.32,0.48,0.64,0.8,0.96];
        m.forEach((f,i)=>{
            tone(f,'sine',0.22,0.5,t[i]);
            tone(f*0.5,'triangle',0.22,0.2,t[i]);
        });
        // Chord cuối
        [523,659,784,1047].forEach(f=>tone(f,'sine',1,0.35,1.15));
        // Tiếng vỡ bong bóng / pháo
        [0,0.25,0.5,0.75,1.0].forEach(d=>noise(0.12,0.06,1.2+d));
    }

    // THUA 😢
    function playLose() {
        stopBGM();
        tone(392,'sawtooth',0.22,0.5);
        tone(349,'sawtooth',0.22,0.45,0.22);
        tone(311,'sawtooth',0.22,0.4,0.44);
        tone(261,'sawtooth',0.5,0.5,0.66);
        noise(0.2,0.06,0.7);
    }

    // HẾT GIỜ ⏰
    function playTimeUp() {
        stopBGM();
        // Chuông báo động
        [880,880,880].forEach((_,i)=>{
            tone(880,'square',0.08,0.4,i*0.15);
            tone(440,'square',0.08,0.2,i*0.15+0.04);
        });
        tone(220,'sawtooth',0.5,0.4,0.5);
    }

    // ĐỒNG HỒ đếm ngược (gọi mỗi giây)
    function playCountdown(s) {
        if (!enabled||s>10) return;
        const f = 380 + (10-s)*45;
        tone(f,'square',0.07,s<=5?0.4:0.25);
    }

    // DÙNG TRỢ GIÚP
    function playLifeline() {
        tone(880,'sine',0.1,0.3);
        tone(1108,'sine',0.1,0.28,0.1);
        tone(1320,'sine',0.18,0.35,0.2);
    }

    // HỒI SINH 💚
    function playRevive() {
        stopBGM();
        [220,277,330,415,523,659,784].forEach((f,i)=>{
            tone(f,'sine',0.3,0.38,i*0.07);
        });
        tone(1047,'sine',0.6,0.45,0.55);
        [523,659,784,1047].forEach((f,i)=>tone(f,'triangle',0.25,0.25,0.9+i*0.07));
    }

    // DỪNG CHƠI
    function playQuit() {
        stopBGM();
        tone(440,'sine',0.1,0.3);
        tone(370,'sine',0.2,0.25,0.12);
        tone(311,'sine',0.3,0.2,0.28);
    }

    // CLICK UI
    function playClick() { tone(650,'sine',0.04,0.15); }

    // ===========================
    // TOGGLE & PERSIST
    // ===========================
    function toggle() {
        enabled = !enabled;
        localStorage.setItem('sound_enabled', enabled);
        if (!enabled) stopBGM();
        return enabled;
    }
    function isEnabled() { return enabled; }

    return {
        startBGM, stopBGM,
        playNewQuestion, playSelect, playCorrect, playWrong,
        playWin, playLose, playTimeUp, playCountdown,
        playLifeline, playRevive, playQuit, playClick,
        toggle, isEnabled
    };
})();
