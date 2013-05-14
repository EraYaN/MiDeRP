enum StatusByteCode { 
		Unknown = 0x00,
		Forward = 0x46, 
		Stop = 0x53, 
		Left = 0x4c, 
		Right = 0x52,
		Turn = 0x54,
		Acknowledged = 0x06, 
		NotAcknowledged = 0x15,
		Enquiry = 0x05,
		MineDetected = 0x07,
		Done = 0x04
};
char buff[1];
bool done;
//bool started;
bool minehere;
bool led;
void setup()
{
  /* add setup code here */
	Serial.begin(9600);
	done = false;
	minehere = false;
	//started = true; // TODO protocol add start instruction
	led = false;
	pinMode(LED_BUILTIN,OUTPUT);
}
void loop()
{
	/*if(!started){
		if(Serial.available()>0){		
			if(Serial.readBytes(buff,1)>0){
				if(Start == (StatusByteCode)buff[1])
					started = true;
	}*/
	if(!done){
		if(Serial.available()>0){		
			if(Serial.readBytes(buff,1)>0){
				StatusByteCode code = (StatusByteCode)buff[0];
				switch(code){
					case Forward:
						Serial.write(Acknowledged);
						minehere = false;
					break;
					case Stop:
						Serial.write(Acknowledged);
					break;
					case Left:
						Serial.write(Acknowledged);
						minehere = false;
					break;
					case Right:
						Serial.write(Acknowledged);
						minehere = false;
					break;
					case Turn:
						Serial.write(Acknowledged);
						minehere = false;
					break;
					case Done:
						Serial.write(Acknowledged);
						done = true;
					break;
					default:
						Serial.write(NotAcknowledged);
					break;
				}
			}
		} else {
			//maybe a mine
			if(random(0,100)<40&&!minehere){
				Serial.write(MineDetected);
				minehere = true;
			} else {
				Serial.write(Enquiry);
				
			}
		}
	}
	delay(1000);
	if(led){
		digitalWrite(LED_BUILTIN,LOW);
		led=false;
	} else {
		digitalWrite(LED_BUILTIN,HIGH);
		led=true;
	}
}
