import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstalacionDashboardComponent } from './instalacion-dashboard.component';

describe('InstalacionDashboardComponent', () => {
  let component: InstalacionDashboardComponent;
  let fixture: ComponentFixture<InstalacionDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstalacionDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstalacionDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
}); 
