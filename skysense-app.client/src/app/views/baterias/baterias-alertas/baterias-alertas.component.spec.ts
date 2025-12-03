import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BateriasAlertasComponent } from './baterias-alertas.component';

describe('BateriasAlertasComponent', () => {
  let component: BateriasAlertasComponent;
  let fixture: ComponentFixture<BateriasAlertasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BateriasAlertasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BateriasAlertasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
