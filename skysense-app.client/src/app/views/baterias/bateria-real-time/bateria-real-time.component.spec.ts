import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BateriaRealTimeComponent } from './bateria-real-time.component';

describe('BateriaRealTimeComponent', () => {
  let component: BateriaRealTimeComponent;
  let fixture: ComponentFixture<BateriaRealTimeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BateriaRealTimeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BateriaRealTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
