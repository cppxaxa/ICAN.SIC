function VoiceInit(){
	var voices = window.speechSynthesis.getVoices();
}

function VoiceInfo(){
	result = [];
	speechSynthesis.getVoices().forEach(function(voice) {
	  console.log(voice.name, voice.default ? voice.default :'');
	  result.push(voice.name);
	});
	return result;
}

function Say(voiceIndex, content)
{
	msgRate = 1;
	
	if (content.length > 85)
		msgRate = 0.7;
	
	var msg = new SpeechSynthesisUtterance();
	var voices = window.speechSynthesis.getVoices();
	msg.voice = voices[voiceIndex]; // Note: some voices don't support altering params
	msg.voiceURI = 'native';
	msg.volume = 1; // 0 to 1
	msg.rate = msgRate; // 0.1 to 10
	msg.pitch = 2; //0 to 2
	msg.text = content;
	msg.lang = 'en-US';

	msg.onend = function(e) {
	  console.log('Finished in ' + event.elapsedTime + ' seconds.');
	};

	speechSynthesis.speak(msg);
}