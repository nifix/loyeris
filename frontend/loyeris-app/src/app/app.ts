import { Component } from '@angular/core';
import { Auth } from './pages/auth/auth';

@Component({
  selector: 'app-root',
  imports: [Auth],
  templateUrl: './app.html',
  styleUrl: './app.css',
})

export class App {}
