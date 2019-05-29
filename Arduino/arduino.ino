int relay_pins[] = {7,8}; 

void setup() {
  Serial.begin(115200);  
  for (int i=0; i<2; i++) {
  delay(100);
  pinMode(relay_pins[i], OUTPUT);  
  digitalWrite(relay_pins[i],HIGH);
   }
}

void loop() {
   if (Serial.available() > 0) {
       char receivedChar = Serial.read();
       switch (receivedChar) {
        case 'n':
          digitalWrite(relay_pins[0],LOW);
        break;
        case 'f':
          digitalWrite(relay_pins[0],HIGH);
        break;
        case 'c':
          digitalWrite(relay_pins[1],LOW);
          delay(50);
          digitalWrite(relay_pins[1],HIGH);
        break;
       }
   }
}


