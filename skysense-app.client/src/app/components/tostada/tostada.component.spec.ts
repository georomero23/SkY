import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TostadaComponent } from './tostada.component';

describe('TostadaComponent', () => {
  let component: TostadaComponent;
  let fixture: ComponentFixture<TostadaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TostadaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TostadaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
