using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carMenu : MonoBehaviour {

	GameObject car;

	public void SetCar(GameObject selectedCar){
		car=selectedCar;
	}



	//                                                                  //  OUTSIDE  //



	public void SetHeadLight(bool enable){//                             Headlight enable
		car.GetComponent<carControl>().SetHeadLight(enable);
	}

	public void SetBackLight(bool enable){//                             Backlight enable
		car.GetComponent<carControl>().SetBackLight(enable);
	}

	public void SetReverseLight(bool enable){//                             Reverselight enable
		car.GetComponent<carControl>().SetReverseLight(enable);
	}

	public void LeftHeadLightDestroy(bool destroy){//                             Destroy left headlight
		car.GetComponent<carControl>().LeftHeadLightDestroy(destroy);
	}

	public void RightHeadLightDestroy(bool destroy){//                             Destroy right headlight
		car.GetComponent<carControl>().RightHeadLightDestroy(destroy);
	}

	public void LeftBackLightDestroy(bool destroy){//                             Destroy left backlight
		car.GetComponent<carControl>().LeftBackLightDestroy(destroy);
	}

	public void RightBackLightDestroy(bool destroy){//                             Destroy right backlight
		car.GetComponent<carControl>().RightBackLightDestroy(destroy);
	}

	public void LeftReverseLightDestroy(bool destroy){//                             Destroy right reverselight
		car.GetComponent<carControl>().LeftReverseLightDestroy(destroy);
	}

	public void RightReverseLightDestroy(bool destroy){//                             Destroy right reverselight
		car.GetComponent<carControl>().RightReverseLightDestroy(destroy);
	}

	public void SetTurnSelect(int select){//                                          set turn signal
		car.GetComponent<carControl>().SetTurnSelect(select);
	}

	public void TurnFrontLeftDestroy(bool destroy){//                                Destroy front left turn signal
		car.GetComponent<carControl>().FrontLeftTurnSignalDestroy(destroy);
	}

	public void TurnFrontRightDestroy(bool destroy){//                                Destroy front right turn signal
		car.GetComponent<carControl>().FrontRightTurnSignalDestroy(destroy);
	}

	public void TurnBacktLeftDestroy(bool destroy){//                                Destroy back left turn signal
		car.GetComponent<carControl>().BackLeftTurnSignalDestroy(destroy);
	}

	public void TurnBackRightDestroy(bool destroy){//                                Destroy back right turn signal
		car.GetComponent<carControl>().BackRightTurnSignalDestroy(destroy);
	}

	public void OpenHood(bool open){//                         						  open hood
		car.GetComponent<carControl>().hoodOpen=open;
	}

	public void OpenLeftDoor(bool open){//                    						  open left door
		car.GetComponent<carControl>().leftDoorOpen=open;
	}

	public void OpenRightDoor(bool open){//                   						  open right door
		car.GetComponent<carControl>().rightDoorOpen=open;
	}

	public void OpenBackDoor(bool open){//                    						  open back door
		car.GetComponent<carControl>().backDoorOpen=open;
	}




	//                                                                  //  INTSIDE  //






	public void SetSpeed(float speed){//                                          set speed
		car.GetComponent<dashBoard>().SetSpeed(speed);
	}

	public void SetRPM(float rpm){//                                              set rpm
		car.GetComponent<dashBoard>().SetRPM(rpm);
	}

	public void SetFuelLevel(float level){//                                      set level
		car.GetComponent<dashBoard>().SetFuelLevel(level);
	}

	public void SetTemperature(float temperature){//                              set temperature
		car.GetComponent<dashBoard>().SetTemperature(temperature);
	}

	public void SetSteeringWheelAngle(float angle){//                             set steering wheel angle
		car.GetComponent<dashBoard>().SetSteeringWheelAngle(-angle);
	}

	public void OpenGloveCompartment(bool open){
		car.GetComponent<dashBoard>().OpenGloveCompartment(open);//               open glove compartment
	}

	public void EnableDashboardLights(bool enable){
		car.GetComponent<dashBoard>().EnableDashboardLights(enable);//            open glove compartment
	}

	public void SetGear(int gear){//                                              set gear
		car.GetComponent<dashBoard>().SetGear(gear);
	}

	public void SetOil(bool enable){//                                            enable oil icon
		car.GetComponent<dashBoard>().SetOil(enable);
	}

	public void SetBeam(bool enable){//                                           enable beam icon
		car.GetComponent<dashBoard>().SetBeam(enable);
	}

	public void SetCharge(bool enable){//                                         enable charge icon
		car.GetComponent<dashBoard>().SetCharge(enable);
	}

	public void SetFastenBelts(bool enable){//                                    enable fasten belts icon
		car.GetComponent<dashBoard>().SetFastenBelts(enable);
	}

	public void SetBrake(bool enable){//                                          enable brake icon
		car.GetComponent<dashBoard>().SetBrake(enable);
	}

	public void Set4WD(bool enable){//                                            enable 4wd icon
		car.GetComponent<dashBoard>().Set4WD(enable);
	}






	//                                                                        OTHER

	public void SetColorPicker(bool enable){
		car.GetComponent<colorpicker>().enabled=enable;
	}
}
