using GBDotNet.Core;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameBoy
{
    public class GameBoyAudio
    {
        private const int bytesPerSample = 2;
        private readonly APU apu;
        private readonly byte[] sampleBuffer;
        private readonly DynamicSoundEffectInstance sound;

        public GameBoyAudio(APU apu)
        {
            this.apu = apu;
            sampleBuffer = new byte[apu.SampleBuffer.Length * bytesPerSample];
            sound = new DynamicSoundEffectInstance(APU.PlaybackSampleRate, AudioChannels.Mono);
        }

        public void Play() => sound.Play();
        public void Mute() => sound.Volume = 0;

        public void Update()
        {
            while (sound.PendingBufferCount < 3)
            {
                SubmitBuffer();
            }
        }

        private void LoadSampleBufferFromApu()
        {
            //byte -> short samples
            for(int i = 0; i < apu.SampleBuffer.Length; i++)
            {
                byte sample = apu.SampleBuffer[i];
                sampleBuffer[i] = sample;
                sampleBuffer[i + 1] = 0;
            }
            apu.RestartSampleBuffer();
        }

        private void SubmitBuffer()
        {
            //LoadSampleBufferFromApu();
            //sound.SubmitBuffer(sampleBuffer);
            sound.SubmitBuffer(apu.SampleBuffer);
        }
    }
}
