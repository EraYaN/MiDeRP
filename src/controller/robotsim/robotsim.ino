enum StatusByteCode { 
		Unknown = 0x00,
		Continue = 0x01,
		Back = 0x42,
		Forward = 0x46,
		Stop = 0x53,
		Left = 0x4c,
		Right = 0x52,
		Turn = 0x54,
		Acknowledged = 0x06,
		NotAcknowledged = 0x15,
		Halfway = 0x48,
		Enquiry = 0x05,
		MineDetected = 0x07,
		Done = 0x04
};
char buff[1];
bool done;
//bool started;
bool minehere;
bool led;
bool waiting;
void setup()
{
  /* add setup code here */
	Serial.begin(9600);
	done = false;
	minehere = false;
	//started = true; // TODO protocol add start instruction
	led = false;
	waiting = false;
	pinMode(LED_BUILTIN,OUTPUT);
}
void loop()
{
	if(!done){
		if(Serial.available()>0){		
			if(Serial.readBytes(buff,1)>0){
				StatusByteCode code = (StatusByteCode)buff[0];
				switch(code){
					case Forward:
						Serial.write(Acknowledged);
						minehere = false;
						waiting = false;
						delay(200);
						Serial.write(Halfway);
					break;
					case Stop:
						waiting = false;
						Serial.write(Acknowledged);
					break;
					case Left:
						waiting = false;
						Serial.write(Acknowledged);
						minehere = false;
						delay(200);
						Serial.write(Halfway);
					break;
					case Right:
						waiting = false;
						Serial.write(Acknowledged);
						minehere = false;
						delay(200);
						Serial.write(Halfway);
					break;
					case Turn:
						waiting = false;
						Serial.write(Acknowledged);
						minehere = false;
						delay(200);
						Serial.write(Halfway);
					break;
					case Back:
						waiting = false;
						Serial.write(Acknowledged);
						minehere = false;
					case Done:
						waiting = false;
						Serial.write(Acknowledged);
						done = true;
					break;
					case Continue:
						waiting = false;
						Serial.write(Acknowledged);
					break;
					case Acknowledged:
					break;
					default:
						Serial.write(NotAcknowledged);
					break;
				}
			}
		} else {
			//maybe a mine
			if(random(0,100)<20&&!minehere){
				Serial.write(MineDetected);
				delay(100);
				Serial.write(Enquiry);
				waiting = true;
				minehere = true;
			} else {
				if(!waiting){
					Serial.write(Enquiry);	
					waiting = true;
				}
			}
		}
	}
	delay(200);
	if(led){
		digitalWrite(LED_BUILTIN,LOW);
		led=false;
	} else {
		digitalWrite(LED_BUILTIN,HIGH);
		led=true;
	}
}
