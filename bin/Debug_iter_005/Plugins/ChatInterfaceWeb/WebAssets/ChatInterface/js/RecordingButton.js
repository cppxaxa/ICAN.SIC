function StartRecordingAnimation(el){
	$(el).removeClass("notRec");
	$(el).addClass("Rec");
}

function EndRecordingAnimation(el){
	$(el).removeClass("Rec");
	$(el).addClass("notRec");
}