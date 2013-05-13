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
void setup()
{
  /* add setup code here */
	Serial.begin(9600);
	done = false;
	minehere = false;
	//started = true; // TODO protocol add start instruction
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
						Serial.print(Acknowledged);
						minehere = false;
					break;
					case Stop:
						Serial.print(Acknowledged);
					break;
					case Left:
						Serial.print(Acknowledged);
						minehere = false;
					break;
					case Right:
						Serial.print(Acknowledged);
						minehere = false;
					break;
					case Turn:
						Serial.print(Acknowledged);
						minehere = false;
					break;
					case Done:
						Serial.print(Acknowledged);
						done = true;
					break;
					default:
						Serial.print(NotAcknowledged);
					break;
				}
			}
		} else {
			//maybe a mine
			if(random(0,100)<10&&!minehere){
				Serial.print(MineDetected);
				minehere = true;
			} else {
				Serial.print(Enquiry);
				
			}
		}
	}
	delay(1000);
}
