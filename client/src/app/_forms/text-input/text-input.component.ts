import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css'],
})

//ControlValueAccessor acts as a bridge between
//angular forms API
//and the native element in the DOM
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  //@self says we are going to inject something
  //more about @self
  //in angular when we inject something
  //it checks whether it has been used recently
  //and tries to fetch from there (memory)
  //self tries not to reuse any controller for the inout

  //NgControl is the base class where all form control
  //based directives extend
  //binds form control object to DOM element
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this; //represents text input component class
  }
  //these methods will be controlled by the
  //form controller
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  get control(): FormControl {
    //casting to get around the type script error
    //get is a keyword which is used
    //to get when we access the control
    return this.ngControl.control as FormControl;
  }
}
