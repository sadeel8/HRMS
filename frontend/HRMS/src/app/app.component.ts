import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf,NgFor,NgClass,NgStyle } from '@angular/common';
import { FormsModule,FormGroup,FormControl,ReactiveFormsModule,Validators } from '@angular/forms';
//FormGroup,FormControl==>class or interface
//Decorator
@Component({
  //components,directives,pipes,module
  imports: [RouterOutlet, NgIf ,NgFor,NgClass,NgStyle,FormsModule,ReactiveFormsModule ],
  selector: 'app-root',
  styleUrl: './app.component.css',
  templateUrl: './app.component.html',
})
export class App {

form=new FormGroup({
  // عشان اربطهم مع بعض html موجودين داخل input or select لكل  FormControl نعرف 
  //minLength==>string,//max==>number
  name: new FormControl(null,Validators.required),
  email: new FormControl(null,[Validators.required,Validators.email]),
  phone: new FormControl(null,[Validators.required,Validators.maxLength(10),Validators.minLength(9)]) ,
  age: new FormControl(null,[Validators.max(35),Validators.min(18)]),
  courseId: new FormControl(1,Validators.required)

});
courses=[
  {id:1,name:"Asp.Net"},
  {id:2,name:"Angular"},
  {id:3,name:"Python"},
  {id:4,name:"Java"},
]

submit(){
  let name=this.form.value.name;
  let courseId=this.form.value.courseId;
  let courseName=this.courses.find(x => x.id==courseId)?.name
  alert(`Welcome ${name} to the academy .
    we will contact you shortly about the ${courseName}course. `);
}



resetform(){
  this.form.reset({
    courseId:1
  });
}





















  //  title="Welcome to Angular From TypeScript";
  //  number=256.5164;
  //  bool=true;
  //  arr=["one","two"];


  //  stu={name:'stu1',mark:88};

  //  students=[
  //   {id:1 ,name:'stu1',mark:88},
  //   {id:2,name:'stu2',mark:49},
  //   {id:3,name:'stu3',mark:58},
  //   {id:4,name:'stu4',mark:33},
  //   {id:5,name:'stu5',mark:99},
  //   {id:6,name:'stu6',mark:66},
  //  ];



  //  images = [
  //   "https://i0.wp.com/picjumbo.com/wp-content/uploads/beautiful-fall-nature-scenery-free-image.jpeg?w=2210&quality=70",
  //   "https://t3.ftcdn.net/jpg/02/70/35/00/360_F_270350073_WO6yQAdptEnAhYKM5GuA9035wbRnVJSr.jpg",
  //   "https://images.pexels.com/photos/26151151/pexels-photo-26151151/free-photo-of-night-sky-filled-with-stars-reflecting-in-the-lake.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500"
  // ];
  // currentIndex=0;
  // name="emp1";

  
  // next(){
  //   let temp=0;
  //   temp++;
  //   this.currentIndex++;
  // }
  // previous(){
  //   this.currentIndex--;
  // }
  // temp(){
  //   let x=15;
  //   // x="string";
  //   x=8;
  //   let y:string="s";
  //    y="10";
  //    y=8;
  // }

  // temp(x:number,y:boolean):string{
  //   return"";

  // }
}
